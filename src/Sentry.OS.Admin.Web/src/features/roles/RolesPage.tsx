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
import { useActiveOrganization } from "@/features/shell/ActiveOrganizationProvider";
import { RoleScopesDialog } from "@/features/roles/RoleScopesDialog";
import {
  useCreateRole,
  useDeleteRole,
  useRoles,
  useUpdateRole,
  type Role,
} from "@/features/roles/api";

type DialogState = { mode: "create" } | { mode: "edit"; role: Role } | null;

const EMPTY = { name: "", description: "", level: "" };

/** Roles management for the active organization, including scope attach/detach. */
export function RolesPage() {
  const { t } = useTranslation();
  const { activeOrganizationId } = useActiveOrganization();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [scopesFor, setScopesFor] = useState<Role | null>(null);
  const [form, setForm] = useState(EMPTY);

  const { data, isError } = useRoles(activeOrganizationId, page);
  const create = useCreateRole(activeOrganizationId ?? "");
  const update = useUpdateRole(activeOrganizationId ?? "");
  const remove = useDeleteRole(activeOrganizationId ?? "");

  if (!activeOrganizationId) {
    return <FriendlyError message={t("common.errors.unexpected")} />;
  }

  const openCreate = () => {
    setForm(EMPTY);
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (role: Role) => {
    setForm({ name: role.name, description: role.description ?? "", level: role.level?.toString() ?? "" });
    update.reset();
    setDialog({ mode: "edit", role });
  };

  const submit = () => {
    const body = {
      name: form.name,
      description: form.description || null,
      level: form.level.trim() === "" ? null : Number(form.level),
    };
    if (dialog?.mode === "create") {
      create.mutate(body, { onSuccess: () => setDialog(null) });
    } else if (dialog?.mode === "edit") {
      update.mutate({ id: dialog.role.id, body }, { onSuccess: () => setDialog(null) });
    }
  };

  return (
    <div>
      <ListPageHeader
        title={t("roles.list.title")}
        action={<Button onClick={openCreate}>{t("roles.list.create")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(r) => r.id}
        columns={[
          { key: "name", header: t("roles.detail.nameLabel") },
          { key: "level", header: t("roles.detail.levelLabel") },
          { key: "scopes", header: t("roles.detail.scopes") },
          { key: "actions", header: "" },
        ]}
        renderRow={(role) => (
          <TableRow>
            <TableCell className="font-medium">{role.name}</TableCell>
            <TableCell className="text-muted-foreground">{role.level ?? "—"}</TableCell>
            <TableCell>
              <Badge variant="outline">{role.scopeNames.length}</Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="outline" size="sm" onClick={() => setScopesFor(role)}>
                {t("roles.detail.scopes")}
              </Button>
              <Button variant="outline" size="sm" onClick={() => openEdit(role)}>
                {t("common.actions.edit")}
              </Button>
              <ConfirmDeactivateDialog
                trigger={
                  <Button variant="outline" size="sm">
                    {t("common.actions.delete")}
                  </Button>
                }
                title={t("common.actions.delete")}
                description={role.name}
                isPending={remove.isPending}
                onConfirm={() => remove.mutate(role.id)}
              />
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("roles.list.create")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("roles.detail.nameLabel")} htmlFor="role-name">
          <Input id="role-name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        </Field>
        <Field label="Description" htmlFor="role-desc">
          <Input id="role-desc" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </Field>
        <Field label={t("roles.detail.levelLabel")} htmlFor="role-level" hint={t("roles.detail.levelBlocked")}>
          <Input
            id="role-level"
            type="number"
            value={form.level}
            onChange={(e) => setForm({ ...form, level: e.target.value })}
          />
        </Field>
      </FormDialog>

      {scopesFor && (
        <RoleScopesDialog
          organizationId={activeOrganizationId}
          role={scopesFor}
          open={scopesFor !== null}
          onOpenChange={(open) => !open && setScopesFor(null)}
        />
      )}
    </div>
  );
}
