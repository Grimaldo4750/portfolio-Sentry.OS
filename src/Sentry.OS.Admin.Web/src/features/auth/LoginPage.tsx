import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Navigate, useLocation } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { ThemeSelector } from "@/features/shell/ThemeSelector";
import { LanguageSelector } from "@/features/shell/LanguageSelector";
import { useAuth } from "@/features/auth/AuthProvider";

/**
 * The portal's login screen — the initial view for any unauthenticated visitor (FR-001). It does
 * not collect credentials: the button redirects to the Sentry.OS IdP's hosted login page
 * (Authorization Code + PKCE), which redirects back to /callback.
 */
export function LoginPage() {
  const { t } = useTranslation();
  const { isAuthenticated, isSigningIn, beginSignIn, lastSignOutReason } = useAuth();
  const location = useLocation();
  const [errorMessage, setErrorMessage] = useState<string | undefined>(
    lastSignOutReason === "sessionExpired" ? t("auth.errors.sessionExpired") : undefined,
  );

  if (isAuthenticated) {
    const redirectTo = (location.state as { from?: Location } | null)?.from?.pathname ?? "/";
    return <Navigate to={redirectTo} replace />;
  }

  const returnTo = (location.state as { from?: Location } | null)?.from?.pathname ?? "/";

  const onSignIn = async () => {
    setErrorMessage(undefined);
    try {
      await beginSignIn(returnTo);
    } catch {
      setErrorMessage(t("auth.errors.serviceUnavailable"));
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm rounded-lg border border-border bg-card p-6 shadow-sm">
        <div className="mb-4 flex items-center justify-between gap-2">
          <h1 className="text-lg font-semibold">{t("auth.login.title")}</h1>
          <div className="flex gap-2">
            <LanguageSelector />
            <ThemeSelector />
          </div>
        </div>

        {errorMessage && <FriendlyError message={errorMessage} className="mb-4" />}

        <Button type="button" className="w-full" onClick={onSignIn} disabled={isSigningIn} aria-busy={isSigningIn}>
          {isSigningIn ? t("auth.login.submitting") : t("auth.login.submit")}
        </Button>
      </div>
    </div>
  );
}
