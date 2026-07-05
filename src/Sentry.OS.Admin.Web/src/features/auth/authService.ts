import axios from "axios";
import type { Session } from "@/features/auth/session";

/**
 * Speaks to the OIDC authority's token endpoint directly (a mocked authority for this feature
 * — see oidcConfig.ts and specs/003-admin-web-login/plan.md Complexity Tracking). The
 * `oidc-client-ts` `UserManager` remains configured for the real Authorization Code + PKCE
 * redirect flow that will replace this direct call once Sentry.OS.IdentityServer ships a real
 * OIDC surface; until then, the portal's own LoginPage collects credentials and exchanges them
 * here rather than redirecting to a separately hosted IdP login page.
 */
const authorityClient = axios.create({
  baseURL: import.meta.env.VITE_OIDC_AUTHORITY,
});

interface TokenResponse {
  access_token: string;
  id_token: string;
  expires_in: number;
  profile: {
    sub: string;
    name: string;
    email: string;
    organization_id: string;
    global_administrator: boolean;
    role_level: number;
  };
}

export async function signIn(email: string, password: string): Promise<Session> {
  try {
    const { data } = await authorityClient.post<TokenResponse>("/connect/token", {
      grant_type: "password",
      client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
      username: email,
      password,
      scope: "openid profile admin.read admin.write",
    });

    return {
      accessToken: data.access_token,
      idToken: data.id_token,
      expiresAtUtc: new Date(Date.now() + data.expires_in * 1000).toISOString(),
      user: {
        id: data.profile.sub,
        name: data.profile.name,
        email: data.profile.email,
        homeOrganizationId: data.profile.organization_id,
        isGlobalAdministrator: data.profile.global_administrator,
        highestRoleLevel: data.profile.role_level,
      },
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (!error.response) {
        throw new AuthServiceError("connectivity");
      }
      if (error.response.status === 400) {
        throw new AuthServiceError("invalidCredentials");
      }
    }
    throw new AuthServiceError("unknown");
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
