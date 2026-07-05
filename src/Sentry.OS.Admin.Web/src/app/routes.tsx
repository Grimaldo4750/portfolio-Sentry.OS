import { createBrowserRouter } from "react-router-dom";
import { LoginPage } from "@/features/auth/LoginPage";
import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { AppShell } from "@/features/shell/AppShell";
import { DashboardPage } from "@/features/shell/DashboardPage";

/**
 * Public `/login` plus a protected shell route tree. Each user story registers its own
 * children under the protected `AppShell` route as it's implemented (see tasks.md).
 */
export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppShell />,
        children: [{ index: true, element: <DashboardPage /> }],
      },
    ],
  },
]);
