import { useMutation, useQueries, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiClient, unwrap } from "@/lib/apiClient";
import type { ApiResponse, PagedResult } from "@/lib/apiResponse";

// ---------------------------------------------------------------------------- Applications

export interface Application {
  id: string;
  organizationId: string;
  name: string;
  slug: string;
  description: string | null;
  isActive: boolean;
}

const appsBase = (organizationId: string) => `/api/organizations/${organizationId}/applications`;

export function useApplications(organizationId: string | undefined, page = 1, pageSize = 200) {
  return useQuery({
    queryKey: ["applications", organizationId, page, pageSize],
    enabled: !!organizationId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<Application>>>(appsBase(organizationId!), { params: { page, pageSize } })),
  });
}

export function useCreateApplication(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: { name: string; slug: string; description: string | null }) =>
      unwrap(apiClient.post<ApiResponse<Application>>(appsBase(organizationId), body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["applications", organizationId] }),
  });
}

export function useUpdateApplication(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: { name: string; description: string | null } }) =>
      unwrap(apiClient.put<ApiResponse<Application>>(`${appsBase(organizationId)}/${id}`, body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["applications", organizationId] }),
  });
}

export function useDeactivateApplication(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) =>
      unwrap(apiClient.post<ApiResponse<Application>>(`${appsBase(organizationId)}/${id}/deactivate`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["applications", organizationId] }),
  });
}

// ---------------------------------------------------------------------------- API Resources & Scopes

export interface Scope {
  id: string;
  apiResourceId: string;
  name: string;
  displayName: string;
  description: string | null;
}

export interface ApiResource {
  id: string;
  applicationId: string;
  name: string;
  displayName: string;
  isActive: boolean;
  scopes: Scope[];
}

const resourcesBase = (applicationId: string) => `/api/applications/${applicationId}/resources`;
const scopesBase = (apiResourceId: string) => `/api/resources/${apiResourceId}/scopes`;

export function useApiResources(applicationId: string | undefined, page = 1, pageSize = 200) {
  return useQuery({
    queryKey: ["apiResources", applicationId, page, pageSize],
    enabled: !!applicationId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<ApiResource>>>(resourcesBase(applicationId!), { params: { page, pageSize } })),
  });
}

export function useCreateApiResource(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: { name: string; displayName: string }) =>
      unwrap(apiClient.post<ApiResponse<ApiResource>>(resourcesBase(applicationId), body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["apiResources", applicationId] }),
  });
}

export function useUpdateApiResource(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: { displayName: string } }) =>
      unwrap(apiClient.put<ApiResponse<ApiResource>>(`${resourcesBase(applicationId)}/${id}`, body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["apiResources", applicationId] }),
  });
}

export function useDeleteApiResource(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => unwrap(apiClient.delete<ApiResponse<ApiResource>>(`${resourcesBase(applicationId)}/${id}`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["apiResources", applicationId] }),
  });
}

export function useScopes(apiResourceId: string | undefined, page = 1, pageSize = 200) {
  return useQuery({
    queryKey: ["scopes", apiResourceId, page, pageSize],
    enabled: !!apiResourceId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<Scope>>>(scopesBase(apiResourceId!), { params: { page, pageSize } })),
  });
}

export function useCreateScope(apiResourceId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: { name: string; displayName: string; description: string | null }) =>
      unwrap(apiClient.post<ApiResponse<Scope>>(scopesBase(apiResourceId), body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["scopes", apiResourceId] }),
  });
}

export function useUpdateScope(apiResourceId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: { displayName: string; description: string | null } }) =>
      unwrap(apiClient.put<ApiResponse<Scope>>(`${scopesBase(apiResourceId)}/${id}`, body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["scopes", apiResourceId] }),
  });
}

export function useDeleteScope(apiResourceId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => unwrap(apiClient.delete<ApiResponse<Scope>>(`${scopesBase(apiResourceId)}/${id}`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["scopes", apiResourceId] }),
  });
}

// ---------------------------------------------------------------------------- Clients

export interface Client {
  id: string;
  applicationId: string;
  clientId: string;
  displayName: string;
  requirePkce: boolean;
  requireClientSecret: boolean;
  accessTokenLifetimeSeconds: number;
  identityTokenLifetimeSeconds: number;
  refreshTokenLifetimeSeconds: number;
  refreshTokenRotationEnabled: boolean;
  isActive: boolean;
  allowedScopeNames: string[];
}

export interface ClientWrite {
  displayName: string;
  requirePkce: boolean;
  requireClientSecret: boolean;
  accessTokenLifetimeSeconds: number;
  identityTokenLifetimeSeconds: number;
  refreshTokenLifetimeSeconds: number;
  refreshTokenRotationEnabled: boolean;
}

const clientsBase = (applicationId: string) => `/api/applications/${applicationId}/clients`;

export function useClients(applicationId: string | undefined, page = 1, pageSize = 200) {
  return useQuery({
    queryKey: ["clients", applicationId, page, pageSize],
    enabled: !!applicationId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<Client>>>(clientsBase(applicationId!), { params: { page, pageSize } })),
  });
}

export function useCreateClient(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: ClientWrite) => unwrap(apiClient.post<ApiResponse<Client>>(clientsBase(applicationId), body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["clients", applicationId] }),
  });
}

export function useUpdateClient(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: ClientWrite }) =>
      unwrap(apiClient.put<ApiResponse<Client>>(`${clientsBase(applicationId)}/${id}`, body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["clients", applicationId] }),
  });
}

export function useDeactivateClient(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => unwrap(apiClient.post<ApiResponse<Client>>(`${clientsBase(applicationId)}/${id}/deactivate`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["clients", applicationId] }),
  });
}

export function useSetClientScopes(applicationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ clientId, scopeIds }: { clientId: string; scopeIds: string[] }) =>
      unwrap(apiClient.put<ApiResponse<Client>>(`${clientsBase(applicationId)}/${clientId}/scopes`, { scopeIds })),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["clients", applicationId] }),
  });
}

// ---------------------------------------------------------------------------- Org-wide scope catalog

export interface CatalogScope extends Scope {
  applicationName: string;
  apiResourceName: string;
}

/**
 * Flattens every scope defined across the active organization's applications and API resources
 * (resource list responses already carry their scopes inline), for scope pickers on the role and
 * client editors.
 */
export function useOrganizationScopes(organizationId: string | undefined) {
  const applications = useApplications(organizationId);
  const apps = applications.data?.items ?? [];

  const resourceQueries = useQueries({
    queries: apps.map((app) => ({
      queryKey: ["apiResources", app.id, 1, 200] as const,
      enabled: !!organizationId,
      queryFn: () =>
        unwrap(apiClient.get<ApiResponse<PagedResult<ApiResource>>>(resourcesBase(app.id), { params: { page: 1, pageSize: 200 } })),
    })),
  });

  const scopes: CatalogScope[] = [];
  apps.forEach((app, index) => {
    const resources = resourceQueries[index]?.data?.items ?? [];
    for (const resource of resources) {
      for (const scope of resource.scopes) {
        scopes.push({ ...scope, applicationName: app.name, apiResourceName: resource.displayName });
      }
    }
  });

  const isLoading = applications.isLoading || resourceQueries.some((q) => q.isLoading);
  return { scopes, isLoading };
}
