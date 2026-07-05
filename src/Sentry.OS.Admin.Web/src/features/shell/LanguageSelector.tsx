import { useTranslation } from "react-i18next";
import { SUPPORTED_LANGUAGES, type SupportedLanguage } from "@/app/i18n";
import { useUiPreferences } from "@/app/UiPreferencesProvider";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const LANGUAGE_DISPLAY_NAMES: Record<SupportedLanguage, string> = {
  "en-US": "English (US)",
};

/**
 * Language selector — ships with exactly one language at launch, structured so additional
 * languages can be added later without code changes to already-translated screens (FR-007).
 */
export function LanguageSelector() {
  const { t } = useTranslation();
  const { language, setLanguage } = useUiPreferences();

  return (
    <Select value={language} onValueChange={(value) => setLanguage(value as SupportedLanguage)}>
      <SelectTrigger aria-label={t("shell.languageSelector.label")} className="w-auto min-w-32">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {SUPPORTED_LANGUAGES.map((code) => (
          <SelectItem key={code} value={code}>
            {LANGUAGE_DISPLAY_NAMES[code]}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
