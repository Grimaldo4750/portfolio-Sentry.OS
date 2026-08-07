import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Field } from "@/components/ui/Field";
import { FormDialog } from "@/components/ui/FormDialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { ListPageHeader } from "@/components/ui/ListPageHeader";
import { PagedTable } from "@/components/ui/PagedTable";
import { ConfirmDeactivateDialog } from "@/components/ui/ConfirmDeactivateDialog";
import { TableCell, TableRow } from "@/components/ui/table";
import {
  useCreateScope,
  useDeleteScope,
  useScopes,
  useUpdateScope,
  type Scope,
} from "@/features/applications/api";

type DialogState = { mode: "create" } | { mode: "edit"; scope: Scope } | null;

const EMPTY = { name: "", displayName: "", description: "" };

/** Scopes belonging to an API resource. */
export function ApiResourceScopesPage() {
  const { t } = useTranslation();
  const { applicationId = "", apiResourceId = "" } = useParams();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [form, setForm] = useState(EMPTY);

  const { data, isError } = useScopes(apiResourceId, page);
  const create = useCreateScope(apiResourceId);
  const update = useUpdateScope(apiResourceId);
  const remove = useDeleteScope(apiResourceId);

  const openCreate = () => {
    setForm(EMPTY);
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (scope: Scope) => {
    setForm({ name: scope.name, displayName: scope.displayName, description: scope.description ?? "" });
    update.reset();
    setDialog({ mode: "edit", scope });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate(
        { name: form.name, displayName: form.displayName, description: form.description || null },
        { onSuccess: () => setDialog(null) },
      );
    } else if (dialog?.mode === "edit") {
      update.mutate(
        { id: dialog.scope.id, body: { displayName: form.displayName, description: form.description || null } },
        { onSuccess: () => setDialog(null) },
      );
    }
  };

  return (
    <div>
      <ListPageHeader
        title={t("apiResources.detail.scopes")}
        back={{ to: `/applications/${applicationId}/resources`, label: t("applications.detail.resources") }}
        action={<Button onClick={openCreate}>{t("apiResources.detail.createScope")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(s) => s.id}
        columns={[
          { key: "name", header: t("apiResources.detail.nameLabel") },
          { key: "display", header: "Display name" },
          { key: "description", header: "Description" },
          { key: "actions", header: "" },
        ]}
        renderRow={(scope) => (
          <TableRow>
            <TableCell className="font-medium">{scope.name}</TableCell>
            <TableCell className="text-muted-foreground">{scope.displayName}</TableCell>
            <TableCell className="text-muted-foreground">{scope.description ?? "—"}</TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="outline" size="sm" onClick={() => openEdit(scope)}>
                {t("common.actions.edit")}
              </Button>
              <ConfirmDeactivateDialog
                trigger={
                  <Button variant="outline" size="sm">
                    {t("common.actions.delete")}
                  </Button>
                }
                title={t("common.actions.delete")}
                description={scope.name}
                isPending={remove.isPending}
                onConfirm={() => remove.mutate(scope.id)}
              />
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("apiResources.detail.createScope")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("apiResources.detail.nameLabel")} htmlFor="scope-name">
          <Input
            id="scope-name"
            value={form.name}
            disabled={dialog?.mode === "edit"}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
          />
        </Field>
        <Field label="Display name" htmlFor="scope-display">
          <Input
            id="scope-display"
            value={form.displayName}
            onChange={(e) => setForm({ ...form, displayName: e.target.value })}
            required
          />
        </Field>
        <Field label="Description" htmlFor="scope-desc">
          <Input id="scope-desc" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </Field>
      </FormDialog>
    </div>
  );
}
