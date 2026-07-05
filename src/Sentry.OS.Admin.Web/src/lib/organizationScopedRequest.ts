let activeOrganizationIdProvider: (() => string | undefined) | undefined;

/** Wired by ActiveOrganizationProvider (US4) once the active organization is known. */
export function registerActiveOrganizationIdProvider(provider: () => string | undefined) {
  activeOrganizationIdProvider = provider;
}

/** Returns the currently active organization id, or throws if none is set yet. */
export function getActiveOrganizationId(): string {
  const id = activeOrganizationIdProvider?.();
  if (!id) {
    throw new Error("No active organization is set. Every organization-scoped request requires one (Principle V).");
  }
  return id;
}

/** Builds an organization-scoped path, e.g. `/api/organizations/{id}/users`. */
export function organizationScopedPath(pathSuffix: string): string {
  return `/api/organizations/${getActiveOrganizationId()}${pathSuffix}`;
}
