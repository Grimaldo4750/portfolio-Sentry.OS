import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { useAuth } from "@/features/auth/AuthProvider";
import { completeSignIn } from "@/features/auth/authService";

/**
 * Handles the OIDC redirect back from the IdP: exchanges the authorization code for tokens,
 * establishes the session, and forwards to the originally requested location.
 */
export function CallbackPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { establishSession } = useAuth();
  const [failed, setFailed] = useState(false);
  const ran = useRef(false);

  useEffect(() => {
    // StrictMode double-invokes effects; the authorization code is single-use, so guard it.
    if (ran.current) return;
    ran.current = true;

    void completeSignIn()
      .then(({ session, returnTo }) => {
        establishSession(session);
        navigate(returnTo, { replace: true });
      })
      .catch(() => setFailed(true));
  }, [establishSession, navigate]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      {failed ? (
        <div className="w-full max-w-sm">
          <FriendlyError message={t("auth.errors.serviceUnavailable")} />
          <button
            type="button"
            className="mt-4 text-sm underline"
            onClick={() => navigate("/login", { replace: true })}
          >
            {t("common.actions.back")}
          </button>
        </div>
      ) : (
        <p className="text-sm text-muted-foreground">{t("auth.login.submitting")}</p>
      )}
    </div>
  );
}
