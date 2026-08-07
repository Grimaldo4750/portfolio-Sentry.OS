import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Input } from "@/components/ui/input";
import { Field } from "@/components/ui/Field";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { ListPageHeader } from "@/components/ui/ListPageHeader";
import { PagedTable } from "@/components/ui/PagedTable";
import { TableCell, TableRow } from "@/components/ui/table";
import { formatDateTime } from "@/lib/formatDateTime";
import { useActiveOrganization } from "@/features/shell/ActiveOrganizationProvider";
import { useAuditLog, type AuditLogFilters } from "@/features/audit/api";

/** Read-only audit trail for the active organization, with date-range and entity-type filters. */
export function AuditLogPage() {
  const { t, i18n } = useTranslation();
  const { activeOrganizationId } = useActiveOrganization();
  const [page, setPage] = useState(1);
  const [filters, setFilters] = useState<AuditLogFilters>({});

  const { data, isError } = useAuditLog(activeOrganizationId, page, filters);

  const onFilterChange = (patch: Partial<AuditLogFilters>) => {
    setPage(1);
    setFilters((prev) => ({ ...prev, ...patch }));
  };

  return (
    <div>
      <ListPageHeader title={t("auditLog.title")} />

      <div className="mb-4 flex flex-wrap items-end gap-3">
        <Field label={t("auditLog.filters.dateRange")} htmlFor="audit-from">
          <Input
            id="audit-from"
            type="date"
            onChange={(e) => onFilterChange({ fromUtc: e.target.value ? new Date(e.target.value).toISOString() : undefined })}
          />
        </Field>
        <Field label=" " htmlFor="audit-to">
          <Input
            id="audit-to"
            type="date"
            onChange={(e) => onFilterChange({ toUtc: e.target.value ? new Date(e.target.value).toISOString() : undefined })}
          />
        </Field>
        <Field label={t("auditLog.filters.entityType")} htmlFor="audit-type">
          <Input id="audit-type" onChange={(e) => onFilterChange({ targetType: e.target.value || undefined })} />
        </Field>
      </div>

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(e) => e.id}
        columns={[
          { key: "actor", header: t("auditLog.columns.actor") },
          { key: "action", header: t("auditLog.columns.action") },
          { key: "target", header: t("auditLog.columns.target") },
          { key: "timestamp", header: t("auditLog.columns.timestamp") },
        ]}
        renderRow={(entry) => (
          <TableRow>
            <TableCell>{entry.actorDisplay ?? entry.actorUserId ?? "—"}</TableCell>
            <TableCell className="font-medium">{entry.action}</TableCell>
            <TableCell className="text-muted-foreground">
              {entry.targetType ? `${entry.targetType}${entry.targetId ? ` · ${entry.targetId.slice(0, 8)}` : ""}` : "—"}
            </TableCell>
            <TableCell className="text-muted-foreground">{formatDateTime(entry.occurredAtUtc, i18n.language)}</TableCell>
          </TableRow>
        )}
      />
    </div>
  );
}
