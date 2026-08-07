import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { registerAccessTokenProvider, registerUnauthorizedHandler } from "@/lib/apiClient";
import { beginSignIn as beginSignInRequest, clearOidcSession, loadOidcSession } from "@/features/auth/authService";
import { clearSession, loadSession, saveSession, type Session } from "@/features/auth/session";

export type SignOutReason = "userInitiated" | "sessionExpired";

interface AuthContextValue {
  session: Session | undefined;
  isAuthenticated: boolean;
  isHydrating: boolean;
  isSigningIn: boolean;
  beginSignIn: (returnTo?: string) => Promise<void>;
  establishSession: (session: Session) => void;
  signOut: (reason?: SignOutReason) => void;
  lastSignOutReason: SignOutReason | undefined;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | undefined>(() => loadSession());
  const [isHydrating, setIsHydrating] = useState(true);
  const [isSigningIn, setIsSigningIn] = useState(false);
  const [lastSignOutReason, setLastSignOutReason] = useState<SignOutReason | undefined>(undefined);

  // The oidc-client user store is the source of truth; reconcile the fast-loaded local session
  // against it on mount (and drop a stale local session if the OIDC session has expired).
  useEffect(() => {
    let cancelled = false;
    void loadOidcSession()
      .then((oidcSession) => {
        if (cancelled) return;
        if (oidcSession) {
          saveSession(oidcSession);
          setSession(oidcSession);
        } else {
          clearSession();
          setSession(undefined);
        }
      })
      .finally(() => {
        if (!cancelled) setIsHydrating(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const signOut = useCallback((reason: SignOutReason = "userInitiated") => {
    void clearOidcSession();
    clearSession();
    setSession(undefined);
    setLastSignOutReason(reason);
  }, []);

  registerAccessTokenProvider(() => session?.accessToken);
  registerUnauthorizedHandler(() => signOut("sessionExpired"));

  const beginSignIn = useCallback(async (returnTo: string = "/") => {
    setIsSigningIn(true);
    try {
      await beginSignInRequest(returnTo);
    } catch {
      setIsSigningIn(false);
    }
  }, []);

  const establishSession = useCallback((newSession: Session) => {
    saveSession(newSession);
    setSession(newSession);
    setLastSignOutReason(undefined);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: session !== undefined,
      isHydrating,
      isSigningIn,
      beginSignIn,
      establishSession,
      signOut,
      lastSignOutReason,
    }),
    [session, isHydrating, isSigningIn, beginSignIn, establishSession, signOut, lastSignOutReason],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
