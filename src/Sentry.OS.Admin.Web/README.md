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
Email:    admin@sentry.os
Password: Admin#12345
```

## Getting started

```bash
npm install
npm run dev      # http://localhost:5173 — MSW mock OIDC authority active in dev
```

> **Note**: `Sentry.OS.IdentityServer` does not yet implement a real OAuth2/OIDC surface.
> This portal authenticates against a **mocked OIDC authority** (via MSW) that returns the
> seeded admin identity, so frontend development isn't blocked. See
> [plan.md Complexity Tracking](../../specs/003-admin-web-login/plan.md) for the tracked
> deviation and follow-up plan.

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
