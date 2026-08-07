import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiClient, unwrap } from "@/lib/apiClient";
import type { ApiResponse, PagedResult } from "@/lib/apiResponse";

export interface Organization {
  id: string;
  name: string;
  slug: string;
  displayName: string;
  isActive: boolean;
  createdAtUtc: string;
}

export interface OrganizationCreate {
  name: string;
  slug: string;
  displayName: string;
}

export interface OrganizationUpdate {
  name: string;
  displayName: string;
}

const BASE = "/api/organizations";

export function useOrganizations(page: number, pageSize = 50, enabled = true) {
  return useQuery({
    queryKey: ["organizations", page, pageSize],
    enabled,
    queryFn: () =>
      unwrap(apiClient.get<ApiResponse<PagedResult<Organization>>>(BASE, { params: { page, pageSize } })),
  });
}

export function useCreateOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: OrganizationCreate) =>
      unwrap(apiClient.post<ApiResponse<Organization>>(BASE, body)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["organizations"] }),
  });
}

export function useUpdateOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: OrganizationUpdate }) =>
      unwrap(apiClient.put<ApiResponse<Organization>>(`${BASE}/${id}`, body)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["organizations"] }),
  });
}

export function useDeactivateOrganization() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) =>
      unwrap(apiClient.post<ApiResponse<Organization>>(`${BASE}/${id}/deactivate`)),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["organizations"] }),
  });
}
