import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useTranslation } from "react-i18next";
import { Navigate, useLocation } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { ThemeSelector } from "@/features/shell/ThemeSelector";
import { LanguageSelector } from "@/features/shell/LanguageSelector";
import { useAuth } from "@/features/auth/AuthProvider";
import { AuthServiceError } from "@/features/auth/authService";

const loginSchema = z.object({
  email: z.string().email(),
  password: z.string().min(1),
});

type LoginFormValues = z.infer<typeof loginSchema>;

/** The portal's login screen — the initial view for any unauthenticated visitor (FR-001). */
export function LoginPage() {
  const { t } = useTranslation();
  const { isAuthenticated, isSigningIn, signIn, lastSignOutReason } = useAuth();
  const location = useLocation();
  const [errorMessage, setErrorMessage] = useState<string | undefined>(
    lastSignOutReason === "sessionExpired" ? t("auth.errors.sessionExpired") : undefined,
  );

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  if (isAuthenticated) {
    const redirectTo = (location.state as { from?: Location } | null)?.from?.pathname ?? "/";
    return <Navigate to={redirectTo} replace />;
  }

  const onSubmit = handleSubmit(async ({ email, password }) => {
    setErrorMessage(undefined);
    try {
      await signIn(email, password);
    } catch (error) {
      if (error instanceof AuthServiceError) {
        setErrorMessage(
          error.reason === "invalidCredentials"
            ? t("auth.errors.invalidCredentials")
            : t("auth.errors.serviceUnavailable"),
        );
        return;
      }
      setErrorMessage(t("common.errors.unexpected"));
    }
  });

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

        <form onSubmit={onSubmit} noValidate className="flex flex-col gap-4">
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="email">{t("auth.login.emailLabel")}</Label>
            <Input id="email" type="email" autoComplete="username" {...register("email")} />
            {errors.email && <p className="text-xs text-destructive">{errors.email.message}</p>}
          </div>

          <div className="flex flex-col gap-1.5">
            <Label htmlFor="password">{t("auth.login.passwordLabel")}</Label>
            <Input id="password" type="password" autoComplete="current-password" {...register("password")} />
            {errors.password && <p className="text-xs text-destructive">{errors.password.message}</p>}
          </div>

          <Button type="submit" disabled={isSigningIn} aria-busy={isSigningIn}>
            {isSigningIn ? t("auth.login.submitting") : t("auth.login.submit")}
          </Button>
        </form>
      </div>
    </div>
  );
}
