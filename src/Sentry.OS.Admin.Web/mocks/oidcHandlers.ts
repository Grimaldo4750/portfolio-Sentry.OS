import { http, HttpResponse } from "msw";
import {
  SEED_ADMIN_EMAIL,
  SEED_ADMIN_IS_GLOBAL_ADMINISTRATOR,
  SEED_ADMIN_NAME,
  SEED_ADMIN_PASSWORD,
  SEED_ADMIN_ROLE_LEVEL,
  SEED_ADMIN_SUB,
  SEED_ORGANIZATION_ID,
} from "./seedIdentity";

const authority = import.meta.env.VITE_OIDC_AUTHORITY;

/**
 * Mocked OIDC authority for Sentry.OS.Admin.Web until Sentry.OS.IdentityServer ships a real
 * OAuth2/OIDC surface (see specs/003-admin-web-login/plan.md Complexity Tracking). Only the
 * seeded admin identity is recognized. A sentinel email triggers a simulated IdP-unavailable
 * response for testing that error path (FR-003 / edge cases).
 */
export const oidcHandlers = [
  http.get(`${authority}/.well-known/openid-configuration`, () => {
    return HttpResponse.json({
      issuer: authority,
      authorization_endpoint: `${authority}/connect/authorize`,
      token_endpoint: `${authority}/connect/token`,
      userinfo_endpoint: `${authority}/connect/userinfo`,
      jwks_uri: `${authority}/.well-known/jwks.json`,
      response_types_supported: ["code"],
      scopes_supported: ["openid", "profile", "admin.read", "admin.write"],
      grant_types_supported: ["authorization_code", "refresh_token", "password"],
      code_challenge_methods_supported: ["S256"],
    });
  }),

  http.post(`${authority}/connect/token`, async ({ request }) => {
    const body = (await request.json()) as { username?: string; password?: string };

    if (body.username === "unavailable@sentry.os") {
      return HttpResponse.error();
    }

    if (body.username !== SEED_ADMIN_EMAIL || body.password !== SEED_ADMIN_PASSWORD) {
      return HttpResponse.json(
        { error: "invalid_grant", error_description: "Invalid email or password." },
        { status: 400 },
      );
    }

    return HttpResponse.json({
      access_token: `mock-access-token.${SEED_ADMIN_SUB}`,
      id_token: `mock-id-token.${SEED_ADMIN_SUB}`,
      token_type: "Bearer",
      expires_in: 3600,
      scope: "openid profile admin.read admin.write",
      profile: {
        sub: SEED_ADMIN_SUB,
        name: SEED_ADMIN_NAME,
        email: SEED_ADMIN_EMAIL,
        organization_id: SEED_ORGANIZATION_ID,
        global_administrator: SEED_ADMIN_IS_GLOBAL_ADMINISTRATOR,
        role_level: SEED_ADMIN_ROLE_LEVEL,
      },
    });
  }),

  http.get(`${authority}/connect/userinfo`, () => {
    return HttpResponse.json({ sub: SEED_ADMIN_SUB, name: SEED_ADMIN_NAME, email: SEED_ADMIN_EMAIL });
  }),

  http.get(`${authority}/.well-known/jwks.json`, () => {
    return HttpResponse.json({ keys: [] });
  }),
];
