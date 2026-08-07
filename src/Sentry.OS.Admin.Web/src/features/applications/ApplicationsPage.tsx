import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
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
import {
  useApplications,
  useCreateApplication,
  useDeactivateApplication,
  useUpdateApplication,
  type Application,
} from "@/features/applications/api";

type DialogState = { mode: "create" } | { mode: "edit"; app: Application } | null;

const EMPTY = { name: "", slug: "", description: "" };

/** Applications management for the active organization; drill down to API resources and clients. */
export function ApplicationsPage() {
  const { t } = useTranslation();
  const { activeOrganizationId } = useActiveOrganization();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [form, setForm] = useState(EMPTY);

  const { data, isError } = useApplications(activeOrganizationId, page);
  const create = useCreateApplication(activeOrganizationId ?? "");
  const update = useUpdateApplication(activeOrganizationId ?? "");
  const deactivate = useDeactivateApplication(activeOrganizationId ?? "");

  if (!activeOrganizationId) {
    return <FriendlyError message={t("common.errors.unexpected")} />;
  }

  const openCreate = () => {
    setForm(EMPTY);
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (app: Application) => {
    setForm({ name: app.name, slug: app.slug, description: app.description ?? "" });
    update.reset();
    setDialog({ mode: "edit", app });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate(
        { name: form.name, slug: form.slug, description: form.description || null },
        { onSuccess: () => setDialog(null) },
      );
    } else if (dialog?.mode === "edit") {
      update.mutate(
        { id: dialog.app.id, body: { name: form.name, description: form.description || null } },
        { onSuccess: () => setDialog(null) },
      );
    }
  };

  return (
    <div>
      <ListPageHeader
        title={t("applications.list.title")}
        action={<Button onClick={openCreate}>{t("applications.list.create")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(a) => a.id}
        columns={[
          { key: "name", header: t("applications.detail.nameLabel") },
          { key: "slug", header: "Slug" },
          { key: "status", header: "Status" },
          { key: "actions", header: "" },
        ]}
        renderRow={(app) => (
          <TableRow>
            <TableCell className="font-medium">{app.name}</TableCell>
            <TableCell className="text-muted-foreground">{app.slug}</TableCell>
            <TableCell>
              <Badge variant={app.isActive ? "secondary" : "outline"}>
                {t(app.isActive ? "common.status.active" : "common.status.inactive")}
              </Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="ghost" size="sm" render={<Link to={`/applications/${app.id}/resources`} />}>
                {t("applications.detail.resources")}
              </Button>
              <Button variant="ghost" size="sm" render={<Link to={`/applications/${app.id}/clients`} />}>
                {t("applications.detail.clients")}
              </Button>
              <Button variant="outline" size="sm" onClick={() => openEdit(app)}>
                {t("common.actions.edit")}
              </Button>
              {app.isActive && (
                <ConfirmDeactivateDialog
                  trigger={
                    <Button variant="outline" size="sm">
                      {t("common.actions.deactivate")}
                    </Button>
                  }
                  title={t("common.actions.deactivate")}
                  description={app.name}
                  isPending={deactivate.isPending}
                  onConfirm={() => deactivate.mutate(app.id)}
                />
              )}
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("applications.list.create")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("applications.detail.nameLabel")} htmlFor="app-name">
          <Input id="app-name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        </Field>
        {dialog?.mode === "create" && (
          <Field label="Slug" htmlFor="app-slug">
            <Input id="app-slug" value={form.slug} onChange={(e) => setForm({ ...form, slug: e.target.value })} required />
          </Field>
        )}
        <Field label="Description" htmlFor="app-desc">
          <Input id="app-desc" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </Field>
      </FormDialog>
    </div>
  );
}
