import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiClient, unwrap } from "@/lib/apiClient";
import type { ApiResponse, PagedResult } from "@/lib/apiResponse";

export interface Role {
  id: string;
  organizationId: string;
  name: string;
  description: string | null;
  level: number | null;
  scopeNames: string[];
}

export interface RoleWrite {
  name: string;
  description: string | null;
  level: number | null;
}

const base = (organizationId: string) => `/api/organizations/${organizationId}/roles`;

export function useRoles(organizationId: string | undefined, page = 1, pageSize = 200) {
  return useQuery({
    queryKey: ["roles", organizationId, page, pageSize],
    enabled: !!organizationId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<Role>>>(base(organizationId!), { params: { page, pageSize } })),
  });
}

export function useCreateRole(organizationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: RoleWrite) => unwrap(apiClient.post<ApiResponse<Role>>(base(organizationId), body)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles", organizationId] }),
  });
}

export function useUpdateRole(organizationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: RoleWrite }) =>
      unwrap(apiClient.put<ApiResponse<Role>>(`${base(organizationId)}/${id}`, body)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles", organizationId] }),
  });
}

export function useDeleteRole(organizationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => unwrap(apiClient.delete<ApiResponse<Role>>(`${base(organizationId)}/${id}`)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles", organizationId] }),
  });
}

export function useAttachRoleScope(organizationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ roleId, scopeId }: { roleId: string; scopeId: string }) =>
      unwrap(apiClient.post<ApiResponse<Role>>(`${base(organizationId)}/${roleId}/scopes`, { scopeId })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles", organizationId] }),
  });
}

export function useDetachRoleScope(organizationId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ roleId, scopeId }: { roleId: string; scopeId: string }) =>
      unwrap(apiClient.delete<ApiResponse<Role>>(`${base(organizationId)}/${roleId}/scopes/${scopeId}`)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles", organizationId] }),
  });
}
