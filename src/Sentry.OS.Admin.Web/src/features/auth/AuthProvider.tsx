import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { registerAccessTokenProvider, registerUnauthorizedHandler } from "@/lib/apiClient";
import { AuthServiceError, signIn as signInRequest } from "@/features/auth/authService";
import { clearSession, loadSession, saveSession, type Session } from "@/features/auth/session";

export type LoginErrorReason = "invalidCredentials" | "connectivity" | "unknown";

interface AuthContextValue {
  session: Session | undefined;
  isAuthenticated: boolean;
  isSigningIn: boolean;
  signIn: (email: string, password: string) => Promise<void>;
  signOut: (reason?: "userInitiated" | "sessionExpired") => void;
  lastSignOutReason: "userInitiated" | "sessionExpired" | undefined;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | undefined>(() => loadSession());
  const [isSigningIn, setIsSigningIn] = useState(false);
  const [lastSignOutReason, setLastSignOutReason] = useState<"userInitiated" | "sessionExpired" | undefined>(
    undefined,
  );

  const signOut = useCallback((reason: "userInitiated" | "sessionExpired" = "userInitiated") => {
    clearSession();
    setSession(undefined);
    setLastSignOutReason(reason);
  }, []);

  registerAccessTokenProvider(() => session?.accessToken);
  registerUnauthorizedHandler(() => signOut("sessionExpired"));

  const signIn = useCallback(async (email: string, password: string) => {
    setIsSigningIn(true);
    try {
      const newSession = await signInRequest(email, password);
      saveSession(newSession);
      setSession(newSession);
      setLastSignOutReason(undefined);
    } catch (error) {
      if (error instanceof AuthServiceError) {
        throw error;
      }
      throw new AuthServiceError("unknown");
    } finally {
      setIsSigningIn(false);
    }
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: session !== undefined,
      isSigningIn,
      signIn,
      signOut,
      lastSignOutReason,
    }),
    [session, isSigningIn, signIn, signOut, lastSignOutReason],
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
