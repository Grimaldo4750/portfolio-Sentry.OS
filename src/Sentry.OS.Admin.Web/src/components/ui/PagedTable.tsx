import { cloneElement, type ReactElement } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";

interface PagedTableProps<T> {
  items: T[];
  columns: { key: string; header: string }[];
  renderRow: (item: T) => ReactElement;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  rowKey: (item: T) => string;
}

/** Generic paged table used by every entity list screen. */
export function PagedTable<T>({
  items,
  columns,
  renderRow,
  page,
  pageSize,
  totalCount,
  onPageChange,
  rowKey,
}: PagedTableProps<T>) {
  const { t } = useTranslation();
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <div className="flex flex-col gap-3">
      <Table>
        <TableHeader>
          <TableRow>
            {columns.map((column) => (
              <TableHead key={column.key}>{column.header}</TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {items.length === 0 ? (
            <TableRow>
              <TableCell colSpan={columns.length} className="p-4 text-center text-muted-foreground">
                {t("common.table.empty")}
              </TableCell>
            </TableRow>
          ) : (
            items.map((item) => cloneElement(renderRow(item), { key: rowKey(item) }))
          )}
        </TableBody>
      </Table>

      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>{t("common.table.page", { page, totalPages })}</span>
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="sm"
            disabled={page <= 1}
            onClick={() => onPageChange(page - 1)}
          >
            {t("common.table.previous")}
          </Button>
          <Button
            variant="outline"
            size="sm"
            disabled={page >= totalPages}
            onClick={() => onPageChange(page + 1)}
          >
            {t("common.table.next")}
          </Button>
        </div>
      </div>
    </div>
  );
}
