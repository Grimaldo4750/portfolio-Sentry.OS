export type Theme = "light" | "dark";

export const THEME_STORAGE_KEY = "sentry_os_admin_theme";

export function applyTheme(theme: Theme): void {
  document.documentElement.classList.toggle("dark", theme === "dark");
  window.localStorage.setItem(THEME_STORAGE_KEY, theme);
}

export function readStoredTheme(): Theme {
  return window.localStorage.getItem(THEME_STORAGE_KEY) === "dark" ? "dark" : "light";
}
