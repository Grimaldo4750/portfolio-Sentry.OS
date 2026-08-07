import { useTranslation } from "react-i18next";
import { useAuth } from "@/features/auth/AuthProvider";
import { useCurrentUser } from "@/features/auth/useCurrentUser";
import { Button } from "@/components/ui/button";
import { LogOut } from "lucide-react";
import { ThemeSelector } from "@/features/shell/ThemeSelector";
import { LanguageSelector } from "@/features/shell/LanguageSelector";
import { OrganizationSwitcher } from "@/features/shell/OrganizationSwitcher";

/**
 * Profile picture, name, email, active organization, theme selector, language selector,
 * organization switcher, and logout (Principle X mandated Hero Card). Theme/language/switcher
 * controls are mounted here by US2/US4; this skeleton wires the identity + logout pieces (US1).
 */
export function HeroCard() {
  const { t } = useTranslation();
  const { signOut } = useAuth();
  const currentUser = useCurrentUser();

  return (
    <div className="mb-4 flex flex-col gap-3 rounded-lg border border-border bg-card p-4">
      <div className="flex items-center gap-3">
        <div className="flex size-10 shrink-0 items-center justify-center rounded-full bg-muted text-sm font-semibold uppercase text-muted-foreground">
          {currentUser?.name.slice(0, 1) ?? "?"}
        </div>
        <div className="min-w-0">
          <p className="truncate text-sm font-semibold">{currentUser?.name}</p>
          <p className="truncate text-xs text-muted-foreground">{currentUser?.email}</p>
        </div>
      </div>

      <OrganizationSwitcher />

      <div className="flex flex-wrap gap-2">
        <LanguageSelector />
        <ThemeSelector />
      </div>

      <Button variant="outline" size="sm" onClick={() => signOut("userInitiated")} className="justify-start gap-2">
        <LogOut className="size-4" aria-hidden="true" />
        {t("shell.heroCard.logout")}
      </Button>
    </div>
  );
}
