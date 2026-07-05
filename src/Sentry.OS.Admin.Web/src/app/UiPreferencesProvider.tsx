import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import i18n, { type SupportedLanguage } from "@/app/i18n";
import { applyTheme, readStoredTheme, type Theme } from "@/app/theme";

const LANGUAGE_STORAGE_KEY = "sentry_os_admin_language";

interface UiPreferencesContextValue {
  theme: Theme;
  language: SupportedLanguage;
  setTheme: (theme: Theme) => void;
  setLanguage: (language: SupportedLanguage) => void;
  /** True once an organization default has been applied because the user has no personal override (US10). */
  hasPersonalThemeOverride: boolean;
  hasPersonalLanguageOverride: boolean;
}

const UiPreferencesContext = createContext<UiPreferencesContextValue | undefined>(undefined);

function readStoredLanguage(): SupportedLanguage {
  const stored = window.localStorage.getItem(LANGUAGE_STORAGE_KEY);
  return stored === "en-US" ? "en-US" : "en-US";
}

export function UiPreferencesProvider({ children }: { children: ReactNode }) {
  const [theme, setThemeState] = useState<Theme>(() => readStoredTheme());
  const [language, setLanguageState] = useState<SupportedLanguage>(() => readStoredLanguage());
  const [hasPersonalThemeOverride, setHasPersonalThemeOverride] = useState(
    () => window.localStorage.getItem("sentry_os_admin_theme") !== null,
  );
  const [hasPersonalLanguageOverride, setHasPersonalLanguageOverride] = useState(
    () => window.localStorage.getItem(LANGUAGE_STORAGE_KEY) !== null,
  );

  const setTheme = useCallback((next: Theme) => {
    applyTheme(next);
    setThemeState(next);
    setHasPersonalThemeOverride(true);
  }, []);

  const setLanguage = useCallback((next: SupportedLanguage) => {
    window.localStorage.setItem(LANGUAGE_STORAGE_KEY, next);
    void i18n.changeLanguage(next);
    setLanguageState(next);
    setHasPersonalLanguageOverride(true);
  }, []);

  const value = useMemo(
    () => ({ theme, language, setTheme, setLanguage, hasPersonalThemeOverride, hasPersonalLanguageOverride }),
    [theme, language, setTheme, setLanguage, hasPersonalThemeOverride, hasPersonalLanguageOverride],
  );

  return <UiPreferencesContext.Provider value={value}>{children}</UiPreferencesContext.Provider>;
}

export function useUiPreferences(): UiPreferencesContextValue {
  const context = useContext(UiPreferencesContext);
  if (!context) {
    throw new Error("useUiPreferences must be used within a UiPreferencesProvider");
  }
  return context;
}
