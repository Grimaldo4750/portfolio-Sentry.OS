import type { User } from "oidc-client-ts";
import { userManager } from "@/features/auth/oidcConfig";
import type { Session } from "@/features/auth/session";

/**
 * Authorization Code + PKCE against the real Sentry.OS.IdentityServer. The portal never collects
 * credentials itself — `beginSignIn` redirects to the IdP's hosted login page; the IdP redirects
 * back to /callback with an authorization code that `completeSignIn` exchanges for tokens.
 */

/** Redirects the browser to the IdP authorize endpoint (starts the flow). */
export async function beginSignIn(returnTo: string): Promise<void> {
  await userManager.signinRedirect({ state: { returnTo } });
}

/** Handles the /callback redirect: completes the code exchange and returns the established session. */
export async function completeSignIn(): Promise<{ session: Session; returnTo: string }> {
  try {
    const user = await userManager.signinRedirectCallback();
    return {
      session: sessionFromOidcUser(user),
      returnTo: (user.state as { returnTo?: string } | undefined)?.returnTo ?? "/",
    };
  } catch {
    throw new AuthServiceError("unknown");
  }
}

/** Returns the current oidc-client session as a portal Session, or undefined if none/expired. */
export async function loadOidcSession(): Promise<Session | undefined> {
  const user = await userManager.getUser();
  if (!user || user.expired) {
    return undefined;
  }
  return sessionFromOidcUser(user);
}

/** Clears the local oidc session (no IdP round-trip; end-session is not implemented server-side). */
export async function clearOidcSession(): Promise<void> {
  await userManager.removeUser();
}

/**
 * Builds the portal Session from an oidc-client User. `sub`/`name`/`email` come from the id_token
 * profile; `organization_id`, `global_administrator`, and `role_level` are claims carried on the
 * ACCESS token only (see JwtTokenService.CreateAccessToken), so they are decoded from it here.
 */
export function sessionFromOidcUser(user: User): Session {
  const accessClaims = decodeJwtPayload(user.access_token);
  const roleLevels = collectRoleLevels(accessClaims["role_level"]);

  return {
    accessToken: user.access_token,
    idToken: user.id_token ?? "",
    expiresAtUtc: new Date((user.expires_at ?? 0) * 1000).toISOString(),
    user: {
      id: user.profile.sub,
      name: (user.profile.name as string | undefined) ?? user.profile.sub,
      email: (user.profile.email as string | undefined) ?? "",
      homeOrganizationId: (accessClaims["organization_id"] as string | undefined) ?? "",
      isGlobalAdministrator: accessClaims["global_administrator"] === "true" || accessClaims["global_administrator"] === true,
      highestRoleLevel: roleLevels.length > 0 ? Math.max(...roleLevels) : 0,
    },
  };
}

/** `role_level` may be absent, a single value, or an array of values. */
function collectRoleLevels(claim: unknown): number[] {
  const values = Array.isArray(claim) ? claim : claim == null ? [] : [claim];
  return values.map((v) => Number(v)).filter((n) => Number.isFinite(n));
}

/** Minimal, dependency-free decode of a JWT payload (no signature check — oidc-client already validated it). */
function decodeJwtPayload(token: string): Record<string, unknown> {
  const payload = token.split(".")[1];
  if (!payload) return {};
  try {
    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => `%${c.charCodeAt(0).toString(16).padStart(2, "0")}`)
        .join(""),
    );
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return {};
  }
}

export type AuthServiceErrorReason = "invalidCredentials" | "connectivity" | "unknown";

export class AuthServiceError extends Error {
  readonly reason: AuthServiceErrorReason;

  constructor(reason: AuthServiceErrorReason) {
    super(reason);
    this.name = "AuthServiceError";
    this.reason = reason;
  }
}
