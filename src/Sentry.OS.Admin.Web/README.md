# Sentry.OS.Admin.Web

The React administrative portal for Sentry.OS — login, permanent left-drawer shell (theme +
language + organization switcher), and organization-scoped management screens over every
entity exposed by `Sentry.OS.Admin.API`. See
[specs/003-admin-web-login](../../specs/003-admin-web-login/spec.md) for the feature spec,
plan, and task breakdown.

## Development seed credentials (non-production)

This portal signs in against the shared platform seed defined in
`src/Sentry.OS.Persistence/Seed/SeedConstants.cs`. These are **development-only** values,
not a real secret:

```
Email:    c_grimaldo@outlook.com
Password: D@ngerdays4750
```

## Getting started

```bash
npm install
npm run dev      # http://localhost:5173
```

The portal authenticates against the real `Sentry.OS.IdentityServer` OAuth2/OIDC surface
(Authorization Code + PKCE) configured via `.env.development`:

- `VITE_OIDC_AUTHORITY=https://localhost/SentryOS` — the IdP behind its reverse proxy.
- `VITE_OIDC_CLIENT_ID=sentry-management-web-app` — the seeded public SPA client.
- `VITE_ADMIN_API_BASE_URL=https://localhost:7088` — the Admin Management API.

Both the IdP and `Sentry.OS.Admin.API` must be running (see the repo root README). Clicking
**Sign in** redirects to the IdP's hosted login page and returns to `/callback` with an
authorization code the portal exchanges for tokens.

> **Testing against a mock authority**: the Vitest and Playwright suites run against a mocked
> OIDC authority (MSW, `mocks/oidcHandlers.ts`). The mock intercepts `VITE_OIDC_AUTHORITY`, so
> it is **off by default** in `npm run dev` and only starts when `VITE_ENABLE_MSW=true` is set
> (see `src/main.tsx`).

## Testing

```bash
npm run test       # Vitest unit/component suite
npm run test:e2e   # Playwright end-to-end suite (against the mock OIDC authority)
```

## Stack

React + Vite + TypeScript + TailwindCSS + shadcn/ui, React Router, TanStack Query, Axios,
React Hook Form + Zod, `oidc-client-ts` (Authorization Code + PKCE), i18next
(`src/locales/en-US.json`), MSW for mocking.

## Original Vite template docs

<details>
<summary>React + TypeScript + Vite</summary>

This template provides a minimal setup to get React working in Vite with HMR and some Oxlint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

</details>
