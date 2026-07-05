import { UserManager, WebStorageStateStore } from "oidc-client-ts";

/**
 * Authorization Code + PKCE against the OIDC authority. During this feature the authority is
 * a mocked authority (MSW, see mocks/oidcHandlers.ts) because Sentry.OS.IdentityServer does not
 * yet implement a real OAuth2/OIDC surface — see specs/003-admin-web-login/plan.md Complexity
 * Tracking. Swapping VITE_OIDC_AUTHORITY to the real IdP later requires no code change here.
 */
export const userManager = new UserManager({
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
  redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URI,
  post_logout_redirect_uri: `${window.location.origin}/login`,
  response_type: "code",
  scope: "openid profile admin.read admin.write",
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  automaticSilentRenew: false,
  loadUserInfo: true,
});
