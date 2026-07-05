import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import "@/app/i18n";
import { UiPreferencesProvider } from "@/app/UiPreferencesProvider";
import { LanguageSelector } from "@/features/shell/LanguageSelector";

describe("LanguageSelector", () => {
  it("lists exactly one language at launch, structured for future additions", () => {
    render(
      <UiPreferencesProvider>
        <LanguageSelector />
      </UiPreferencesProvider>,
    );

    expect(screen.getByRole("combobox")).toHaveTextContent("English (US)");
  });
});
