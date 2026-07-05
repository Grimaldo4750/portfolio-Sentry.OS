import { useAuth } from "@/features/auth/AuthProvider";
import type { SessionUser } from "@/features/auth/session";

/** Exposes the signed-in user's identity plus the claims needed for UI-level authorization checks. */
export function useCurrentUser(): SessionUser | undefined {
  const { session } = useAuth();
  return session?.user;
}
