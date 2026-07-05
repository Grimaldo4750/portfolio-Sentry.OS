const SESSION_STORAGE_KEY = "sentry_os_admin_session";

export interface SessionUser {
  id: string;
  name: string;
  email: string;
  homeOrganizationId: string;
  isGlobalAdministrator: boolean;
  highestRoleLevel: number;
}

export interface Session {
  accessToken: string;
  idToken: string;
  expiresAtUtc: string;
  user: SessionUser;
}

export function loadSession(): Session | undefined {
  const raw = window.sessionStorage.getItem(SESSION_STORAGE_KEY);
  if (!raw) return undefined;

  try {
    const session = JSON.parse(raw) as Session;
    if (new Date(session.expiresAtUtc).getTime() <= Date.now()) {
      clearSession();
      return undefined;
    }
    return session;
  } catch {
    clearSession();
    return undefined;
  }
}

export function saveSession(session: Session): void {
  window.sessionStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
}

export function clearSession(): void {
  window.sessionStorage.removeItem(SESSION_STORAGE_KEY);
}
