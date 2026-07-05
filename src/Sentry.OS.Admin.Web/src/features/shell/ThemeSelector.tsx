import { useTranslation } from "react-i18next";
import { Moon, Sun } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useUiPreferences } from "@/app/UiPreferencesProvider";

/** Light/dark theme toggle, usable on the login screen and throughout the shell (FR-005). */
export function ThemeSelector() {
  const { t } = useTranslation();
  const { theme, setTheme } = useUiPreferences();
  const isDark = theme === "dark";

  return (
    <Button
      type="button"
      variant="outline"
      size="icon"
      aria-label={t("shell.themeSelector.label")}
      onClick={() => setTheme(isDark ? "light" : "dark")}
    >
      {isDark ? <Sun className="size-4" /> : <Moon className="size-4" />}
    </Button>
  );
}
