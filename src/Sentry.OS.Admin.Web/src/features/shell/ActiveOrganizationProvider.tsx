import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { registerActiveOrganizationIdProvider } from "@/lib/organizationScopedRequest";
import { useAuth } from "@/features/auth/AuthProvider";

const STORAGE_KEY = "sentry_os_active_organization";

interface ActiveOrganizationContextValue {
  activeOrganizationId: string | undefined;
  setActiveOrganizationId: (id: string) => void;
}

const ActiveOrganizationContext = createContext<ActiveOrganizationContextValue | undefined>(undefined);

/**
 * Tracks the active organization every organization-scoped request targets (Principle V). Defaults
 * to the signed-in user's home organization; a global administrator can switch to another via the
 * Hero Card switcher, and the choice persists across reloads.
 */
export function ActiveOrganizationProvider({ children }: { children: ReactNode }) {
  const { session } = useAuth();
  const homeOrganizationId = session?.user.homeOrganizationId;

  const [activeOrganizationId, setActiveOrganizationIdState] = useState<string | undefined>(
    () => window.sessionStorage.getItem(STORAGE_KEY) ?? homeOrganizationId,
  );

  // Fall back to the home organization once the session hydrates if nothing was selected/persisted.
  useEffect(() => {
    if (!activeOrganizationId && homeOrganizationId) {
      setActiveOrganizationIdState(homeOrganizationId);
    }
  }, [activeOrganizationId, homeOrganizationId]);

  const setActiveOrganizationId = useCallback((id: string) => {
    window.sessionStorage.setItem(STORAGE_KEY, id);
    setActiveOrganizationIdState(id);
  }, []);

  registerActiveOrganizationIdProvider(() => activeOrganizationId);

  const value = useMemo<ActiveOrganizationContextValue>(
    () => ({ activeOrganizationId, setActiveOrganizationId }),
    [activeOrganizationId, setActiveOrganizationId],
  );

  return <ActiveOrganizationContext.Provider value={value}>{children}</ActiveOrganizationContext.Provider>;
}

export function useActiveOrganization(): ActiveOrganizationContextValue {
  const context = useContext(ActiveOrganizationContext);
  if (!context) {
    throw new Error("useActiveOrganization must be used within an ActiveOrganizationProvider");
  }
  return context;
}
