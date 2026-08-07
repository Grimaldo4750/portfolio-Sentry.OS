import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Field } from "@/components/ui/Field";
import { FormDialog } from "@/components/ui/FormDialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { ListPageHeader } from "@/components/ui/ListPageHeader";
import { PagedTable } from "@/components/ui/PagedTable";
import { ConfirmDeactivateDialog } from "@/components/ui/ConfirmDeactivateDialog";
import { TableCell, TableRow } from "@/components/ui/table";
import {
  useCreateOrganization,
  useDeactivateOrganization,
  useOrganizations,
  useUpdateOrganization,
  type Organization,
} from "@/features/organizations/api";

type DialogState = { mode: "create" } | { mode: "edit"; org: Organization } | null;

/** Organizations management (global administrators only) — the top-level isolation boundary. */
export function OrganizationsPage() {
  const { t } = useTranslation();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const { data, isError } = useOrganizations(page);

  const create = useCreateOrganization();
  const update = useUpdateOrganization();
  const deactivate = useDeactivateOrganization();

  const [form, setForm] = useState({ name: "", slug: "", displayName: "" });

  const openCreate = () => {
    setForm({ name: "", slug: "", displayName: "" });
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (org: Organization) => {
    setForm({ name: org.name, slug: org.slug, displayName: org.displayName });
    update.reset();
    setDialog({ mode: "edit", org });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate(form, { onSuccess: () => setDialog(null) });
    } else if (dialog?.mode === "edit") {
      update.mutate(
        { id: dialog.org.id, body: { name: form.name, displayName: form.displayName } },
        { onSuccess: () => setDialog(null) },
      );
    }
  };

  return (
    <div>
      <ListPageHeader
        title={t("organizations.list.title")}
        action={<Button onClick={openCreate}>{t("organizations.list.create")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(o) => o.id}
        columns={[
          { key: "name", header: t("organizations.detail.nameLabel") },
          { key: "slug", header: "Slug" },
          { key: "status", header: "Status" },
          { key: "actions", header: "" },
        ]}
        renderRow={(org) => (
          <TableRow>
            <TableCell className="font-medium">{org.displayName}</TableCell>
            <TableCell className="text-muted-foreground">{org.slug}</TableCell>
            <TableCell>
              <Badge variant={org.isActive ? "secondary" : "outline"}>
                {t(org.isActive ? "common.status.active" : "common.status.inactive")}
              </Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="outline" size="sm" onClick={() => openEdit(org)}>
                {t("common.actions.edit")}
              </Button>
              {org.isActive && (
                <ConfirmDeactivateDialog
                  trigger={
                    <Button variant="outline" size="sm">
                      {t("common.actions.deactivate")}
                    </Button>
                  }
                  title={t("common.actions.deactivate")}
                  description={org.displayName}
                  isPending={deactivate.isPending}
                  onConfirm={() => deactivate.mutate(org.id)}
                />
              )}
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("organizations.list.create")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("organizations.detail.nameLabel")} htmlFor="org-name">
          <Input id="org-name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        </Field>
        {dialog?.mode === "create" && (
          <Field label="Slug" htmlFor="org-slug">
            <Input id="org-slug" value={form.slug} onChange={(e) => setForm({ ...form, slug: e.target.value })} required />
          </Field>
        )}
        <Field label="Display name" htmlFor="org-display">
          <Input
            id="org-display"
            value={form.displayName}
            onChange={(e) => setForm({ ...form, displayName: e.target.value })}
            required
          />
        </Field>
      </FormDialog>
    </div>
  );
}
