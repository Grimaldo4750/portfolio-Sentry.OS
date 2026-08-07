import { ApiError } from "@/lib/apiResponse";

/**
 * Prefers the API's human-readable envelope message (e.g. "An organization with this name already
 * exists.") when present, otherwise falls back to a generic localized message.
 */
export function toErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof ApiError && error.message) {
    return error.message;
  }
  return fallback;
}
