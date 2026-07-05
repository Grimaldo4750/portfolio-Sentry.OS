import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import enUS from "@/locales/en-US.json";

export const SUPPORTED_LANGUAGES = ["en-US"] as const;
export type SupportedLanguage = (typeof SUPPORTED_LANGUAGES)[number];

void i18n.use(initReactI18next).init({
  resources: {
    "en-US": { translation: enUS },
  },
  lng: "en-US",
  fallbackLng: "en-US",
  interpolation: { escapeValue: false },
});

export default i18n;
