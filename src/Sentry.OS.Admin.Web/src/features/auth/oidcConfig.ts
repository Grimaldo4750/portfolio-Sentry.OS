import { UserManager, WebStorageStateStore } from "oidc-client-ts";

/**
 * Authorization Code + PKCE against the real Sentry.OS.IdentityServer OIDC authority
 * (VITE_OIDC_AUTHORITY, e.g. https://localhost/SentryOS). The requested scopes are the seven
 * management scopes the seeded `sentry-management-web-app` client is allowed; `openid`/`profile`
 * are included for OIDC conformance and are simply filtered out of the granted set server-side
 * (the IdP grants the intersection of user-role, client-allowed, and requested scopes).
 */
export const MANAGEMENT_SCOPES = [
  "organizations.manage",
  "applications.manage",
  "resources.manage",
  "clients.manage",
  "roles.manage",
  "users.manage",
  "audit.read",
] as const;

export const userManager = new UserManager({
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
  redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URI,
  post_logout_redirect_uri: `${window.location.origin}/login`,
  response_type: "code",
  scope: ["openid", "profile", ...MANAGEMENT_SCOPES].join(" "),
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  automaticSilentRenew: false,
  loadUserInfo: true,
});
