import { useQuery } from "@tanstack/react-query";
import { apiClient, unwrap } from "@/lib/apiClient";
import type { ApiResponse, PagedResult } from "@/lib/apiResponse";

export interface AuditLogEntry {
  id: string;
  organizationId: string | null;
  actorUserId: string | null;
  actorDisplay: string | null;
  action: string;
  targetType: string | null;
  targetId: string | null;
  occurredAtUtc: string;
}

export interface AuditLogFilters {
  fromUtc?: string;
  toUtc?: string;
  targetType?: string;
}

export function useAuditLog(organizationId: string | undefined, page: number, filters: AuditLogFilters, pageSize = 50) {
  return useQuery({
    queryKey: ["auditLog", organizationId, page, pageSize, filters],
    enabled: !!organizationId,
    queryFn: () =>
      unwrap(
        apiClient.get<ApiResponse<PagedResult<AuditLogEntry>>>(`/api/organizations/${organizationId}/audit-log`, {
          params: {
            page,
            pageSize,
            fromUtc: filters.fromUtc || undefined,
            toUtc: filters.toUtc || undefined,
            targetType: filters.targetType || undefined,
          },
        }),
      ),
  });
}
