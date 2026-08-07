import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "@/features/auth/AuthProvider";

/** Redirects unauthenticated visitors to /login (FR-001, FR-002, and deep-link edge case). */
export function ProtectedRoute() {
  const { isAuthenticated, isHydrating } = useAuth();
  const location = useLocation();

  // Wait for the OIDC session to reconcile before deciding — otherwise a returning user with a
  // valid session but no fast-loaded local copy would be bounced to /login on first paint.
  if (isHydrating) {
    return null;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
}
