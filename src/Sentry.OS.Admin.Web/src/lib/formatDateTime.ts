/**
 * Converts a UTC ISO 8601 timestamp (as returned by the backend, e.g. "2026-07-05T00:00:00Z")
 * to the browser's local time zone for display only (Principle XII presentation boundary).
 */
export function formatDateTime(utcIsoString: string, locale = "en-US"): string {
  const date = new Date(utcIsoString);
  return new Intl.DateTimeFormat(locale, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
}

export function formatDate(utcIsoString: string, locale = "en-US"): string {
  const date = new Date(utcIsoString);
  return new Intl.DateTimeFormat(locale, { dateStyle: "medium" }).format(date);
}
