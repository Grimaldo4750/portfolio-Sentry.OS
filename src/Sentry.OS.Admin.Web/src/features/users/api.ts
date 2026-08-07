import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiClient, unwrap } from "@/lib/apiClient";
import type { ApiResponse, PagedResult } from "@/lib/apiResponse";

export interface User {
  id: string;
  email: string;
  userName: string;
  firstName: string | null;
  lastName: string | null;
  profilePictureUrl: string | null;
  isDisabled: boolean;
  twoFactorEnabled: boolean;
  lastLoginAtUtc: string | null;
  createdAtUtc: string;
}

export interface UserCreate {
  email: string;
  userName: string;
  firstName: string | null;
  lastName: string | null;
}

export interface UserUpdate {
  firstName: string | null;
  lastName: string | null;
  profilePictureUrl: string | null;
}

export interface RoleAssignment {
  roleId: string;
  roleName: string;
  assignedAtUtc: string;
}

const base = (organizationId: string) => `/api/organizations/${organizationId}/users`;

export function useUsers(organizationId: string | undefined, page = 1, pageSize = 50) {
  return useQuery({
    queryKey: ["users", organizationId, page, pageSize],
    enabled: !!organizationId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<User>>>(base(organizationId!), { params: { page, pageSize } })),
  });
}

export function useCreateUser(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UserCreate) => unwrap(apiClient.post<ApiResponse<User>>(base(organizationId), body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["users", organizationId] }),
  });
}

export function useUpdateUser(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UserUpdate }) =>
      unwrap(apiClient.put<ApiResponse<User>>(`${base(organizationId)}/${id}`, body)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["users", organizationId] }),
  });
}

export function useDeactivateUser(organizationId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => unwrap(apiClient.post<ApiResponse<User>>(`${base(organizationId)}/${id}/deactivate`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["users", organizationId] }),
  });
}

export function useUserRoles(organizationId: string | undefined, userId: string | undefined) {
  return useQuery({
    queryKey: ["userRoles", organizationId, userId],
    enabled: !!organizationId && !!userId,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<RoleAssignment[]>>(`${base(organizationId!)}/${userId}/roles`)),
  });
}

export function useAssignUserRole(organizationId: string, userId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (roleId: string) =>
      unwrap(apiClient.post<ApiResponse<RoleAssignment>>(`${base(organizationId)}/${userId}/roles`, { roleId })),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["userRoles", organizationId, userId] }),
  });
}

export function useRemoveUserRole(organizationId: string, userId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (roleId: string) =>
      unwrap(apiClient.delete<ApiResponse<unknown>>(`${base(organizationId)}/${userId}/roles/${roleId}`)),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["userRoles", organizationId, userId] }),
  });
}
