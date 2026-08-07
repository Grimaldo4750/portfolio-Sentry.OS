import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
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
  useApiResources,
  useCreateApiResource,
  useDeleteApiResource,
  useUpdateApiResource,
  type ApiResource,
} from "@/features/applications/api";

type DialogState = { mode: "create" } | { mode: "edit"; resource: ApiResource } | null;

/** API resources belonging to an application; drill down to each resource's scopes. */
export function ApplicationResourcesPage() {
  const { t } = useTranslation();
  const { applicationId = "" } = useParams();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [form, setForm] = useState({ name: "", displayName: "" });

  const { data, isError } = useApiResources(applicationId, page);
  const create = useCreateApiResource(applicationId);
  const update = useUpdateApiResource(applicationId);
  const remove = useDeleteApiResource(applicationId);

  const openCreate = () => {
    setForm({ name: "", displayName: "" });
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (resource: ApiResource) => {
    setForm({ name: resource.name, displayName: resource.displayName });
    update.reset();
    setDialog({ mode: "edit", resource });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate({ name: form.name, displayName: form.displayName }, { onSuccess: () => setDialog(null) });
    } else if (dialog?.mode === "edit") {
      update.mutate({ id: dialog.resource.id, body: { displayName: form.displayName } }, { onSuccess: () => setDialog(null) });
    }
  };

  return (
    <div>
      <ListPageHeader
        title={t("applications.detail.resources")}
        back={{ to: "/applications", label: t("applications.list.title") }}
        action={<Button onClick={openCreate}>{t("apiResources.detail.nameLabel")}</Button>}
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
          { key: "name", header: t("apiResources.detail.nameLabel") },
          { key: "display", header: "Display name" },
          { key: "scopes", header: t("apiResources.detail.scopes") },
          { key: "actions", header: "" },
        ]}
        renderRow={(resource) => (
          <TableRow>
            <TableCell className="font-medium">{resource.name}</TableCell>
            <TableCell className="text-muted-foreground">{resource.displayName}</TableCell>
            <TableCell>
              <Badge variant="outline">{resource.scopes.length}</Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button
                variant="ghost"
                size="sm"
                render={<Link to={`/applications/${applicationId}/resources/${resource.id}/scopes`} />}
              >
                {t("apiResources.detail.scopes")}
              </Button>
              <Button variant="outline" size="sm" onClick={() => openEdit(resource)}>
                {t("common.actions.edit")}
              </Button>
              <ConfirmDeactivateDialog
                trigger={
                  <Button variant="outline" size="sm">
                    {t("common.actions.delete")}
                  </Button>
                }
                title={t("common.actions.delete")}
                description={resource.name}
                isPending={remove.isPending}
                onConfirm={() => remove.mutate(resource.id)}
              />
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("apiResources.detail.nameLabel")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("apiResources.detail.nameLabel")} htmlFor="res-name">
          <Input
            id="res-name"
            value={form.name}
            disabled={dialog?.mode === "edit"}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
          />
        </Field>
        <Field label="Display name" htmlFor="res-display">
          <Input
            id="res-display"
            value={form.displayName}
            onChange={(e) => setForm({ ...form, displayName: e.target.value })}
            required
          />
        </Field>
      </FormDialog>
    </div>
  );
}
