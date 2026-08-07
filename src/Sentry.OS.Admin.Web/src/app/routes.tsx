import { createBrowserRouter } from "react-router-dom";
import { LoginPage } from "@/features/auth/LoginPage";
import { CallbackPage } from "@/features/auth/CallbackPage";
import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { AppShell } from "@/features/shell/AppShell";
import { DashboardPage } from "@/features/shell/DashboardPage";
import { OrganizationsPage } from "@/features/organizations/OrganizationsPage";
import { UsersPage } from "@/features/users/UsersPage";
import { RolesPage } from "@/features/roles/RolesPage";
import { ApplicationsPage } from "@/features/applications/ApplicationsPage";
import { ApplicationResourcesPage } from "@/features/applications/ApplicationResourcesPage";
import { ApplicationClientsPage } from "@/features/applications/ApplicationClientsPage";
import { ApiResourceScopesPage } from "@/features/applications/ApiResourceScopesPage";
import { AuditLogPage } from "@/features/audit/AuditLogPage";

/**
 * Public `/login` and `/callback` plus a protected shell route tree hosting the
 * organization-scoped management screens over every entity `Sentry.OS.Admin.API` exposes.
 */
export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  { path: "/callback", element: <CallbackPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppShell />,
        children: [
          { index: true, element: <DashboardPage /> },
          { path: "organizations", element: <OrganizationsPage /> },
          { path: "users", element: <UsersPage /> },
          { path: "roles", element: <RolesPage /> },
          { path: "applications", element: <ApplicationsPage /> },
          { path: "applications/:applicationId/resources", element: <ApplicationResourcesPage /> },
          { path: "applications/:applicationId/clients", element: <ApplicationClientsPage /> },
          { path: "applications/:applicationId/resources/:apiResourceId/scopes", element: <ApiResourceScopesPage /> },
          { path: "audit-log", element: <AuditLogPage /> },
        ],
      },
    ],
  },
]);
