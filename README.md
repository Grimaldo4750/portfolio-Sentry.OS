# portfolio-Sentry.OS
A decentralized, low-latency IAM and access control kernel designed for securing distributed microservices and multi-organization applications via OAuth2 and OpenID Connect.

## Products

- `src/Sentry.OS.IdentityServer` — the Identity Provider (OAuth 2.0 / OIDC), part of `Sentry.OS.IdentityServer.slnx`.
- `src/Sentry.OS.Admin.API` — the administration REST API, part of `Sentry.OS.Admin.slnx`.
- `src/Sentry.OS.Admin.Web` — the React administrative portal (separate npm project; see [its README](src/Sentry.OS.Admin.Web/README.md)).

## Running the Identity Provider (IdP)

1. Apply the schema and seed to a local SQL Server instance by running `scripts/identity-schema.sql`
   against a `SentryOsIdentity` database (the script is idempotent — safe to re-run). If applying it
   with `sqlcmd` (rather than SSMS, which sets these automatically), first run
   `SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;` in the same session — some of the generated unique
   indexes require them, and `sqlcmd`'s session defaults can differ from SSMS's.
2. Run the IdP: `dotnet run --project src/Sentry.OS.IdentityServer`. It listens on
   `https://localhost:5001` (and `http://localhost:5000`) and, in Development, exposes Swagger UI
   at `/swagger` and its OIDC discovery document at
   `https://localhost:5001/.well-known/openid-configuration`.
3. Sign in with the seeded development credentials:
   - Email: `c_grimaldo@outlook.com`
   - Password: `D@ngerdays4750`
   - **Development-only** — see the comment beside `SeedConstants.AdminPassword` in
     `src/Sentry.OS.Persistence/Seed/SeedConstants.cs`. Not a production secret.

### Development email delivery

Email verification and two-factor authentication links/codes are not sent through a real mail
server in development. `DevelopmentEmailSender` writes every "sent" email — recipient, subject,
and body (including the verification link or one-time code) — to the IdP's console/Serilog output
with a `[DEV EMAIL]` marker. Watch that log to complete either flow locally. A production
deployment supplies a real `IEmailSender` implementation (e.g. SMTP-backed) without any change to
the calling code.
