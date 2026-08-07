import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
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
import { ClientScopesDialog } from "@/features/applications/ClientScopesDialog";
import {
  useClients,
  useCreateClient,
  useDeactivateClient,
  useUpdateClient,
  type Client,
  type ClientWrite,
} from "@/features/applications/api";

type DialogState = { mode: "create" } | { mode: "edit"; client: Client } | null;

const DEFAULTS: ClientWrite = {
  displayName: "",
  requirePkce: true,
  requireClientSecret: false,
  accessTokenLifetimeSeconds: 3600,
  identityTokenLifetimeSeconds: 300,
  refreshTokenLifetimeSeconds: 1209600,
  refreshTokenRotationEnabled: true,
};

/** OAuth clients belonging to an application, including the allowed-scope set editor. */
export function ApplicationClientsPage() {
  const { t } = useTranslation();
  const { applicationId = "" } = useParams();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [scopesFor, setScopesFor] = useState<Client | null>(null);
  const [form, setForm] = useState<ClientWrite>(DEFAULTS);

  const { data, isError } = useClients(applicationId, page);
  const create = useCreateClient(applicationId);
  const update = useUpdateClient(applicationId);
  const deactivate = useDeactivateClient(applicationId);

  const openCreate = () => {
    setForm(DEFAULTS);
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (client: Client) => {
    setForm({
      displayName: client.displayName,
      requirePkce: client.requirePkce,
      requireClientSecret: client.requireClientSecret,
      accessTokenLifetimeSeconds: client.accessTokenLifetimeSeconds,
      identityTokenLifetimeSeconds: client.identityTokenLifetimeSeconds,
      refreshTokenLifetimeSeconds: client.refreshTokenLifetimeSeconds,
      refreshTokenRotationEnabled: client.refreshTokenRotationEnabled,
    });
    update.reset();
    setDialog({ mode: "edit", client });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate(form, { onSuccess: () => setDialog(null) });
    } else if (dialog?.mode === "edit") {
      update.mutate({ id: dialog.client.id, body: form }, { onSuccess: () => setDialog(null) });
    }
  };

  const numberField = (key: keyof ClientWrite, label: string) => (
    <Field label={label} htmlFor={`client-${key}`}>
      <Input
        id={`client-${key}`}
        type="number"
        value={String(form[key])}
        onChange={(e) => setForm({ ...form, [key]: Number(e.target.value) })}
      />
    </Field>
  );

  const checkboxField = (key: "requirePkce" | "requireClientSecret" | "refreshTokenRotationEnabled", label: string) => (
    <label className="flex items-center gap-2 text-sm">
      <input
        type="checkbox"
        checked={form[key]}
        onChange={(e) => setForm({ ...form, [key]: e.target.checked })}
      />
      {label}
    </label>
  );

  return (
    <div>
      <ListPageHeader
        title={t("applications.detail.clients")}
        back={{ to: "/applications", label: t("applications.list.title") }}
        action={<Button onClick={openCreate}>{t("applications.detail.clients")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(c) => c.id}
        columns={[
          { key: "clientId", header: "Client ID" },
          { key: "name", header: t("clients.detail.nameLabel") },
          { key: "status", header: "Status" },
          { key: "actions", header: "" },
        ]}
        renderRow={(client) => (
          <TableRow>
            <TableCell className="font-mono text-xs">{client.clientId}</TableCell>
            <TableCell className="font-medium">{client.displayName}</TableCell>
            <TableCell>
              <Badge variant={client.isActive ? "secondary" : "outline"}>
                {t(client.isActive ? "common.status.active" : "common.status.inactive")}
              </Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="outline" size="sm" onClick={() => setScopesFor(client)}>
                {t("clients.detail.allowedScopes")}
              </Button>
              <Button variant="outline" size="sm" onClick={() => openEdit(client)}>
                {t("common.actions.edit")}
              </Button>
              {client.isActive && (
                <ConfirmDeactivateDialog
                  trigger={
                    <Button variant="outline" size="sm">
                      {t("common.actions.deactivate")}
                    </Button>
                  }
                  title={t("common.actions.deactivate")}
                  description={client.displayName}
                  isPending={deactivate.isPending}
                  onConfirm={() => deactivate.mutate(client.id)}
                />
              )}
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("applications.detail.clients")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("clients.detail.nameLabel")} htmlFor="client-displayName">
          <Input
            id="client-displayName"
            value={form.displayName}
            onChange={(e) => setForm({ ...form, displayName: e.target.value })}
            required
          />
        </Field>
        {checkboxField("requirePkce", "Require PKCE")}
        {checkboxField("requireClientSecret", "Require client secret")}
        {checkboxField("refreshTokenRotationEnabled", "Rotate refresh tokens")}
        {numberField("accessTokenLifetimeSeconds", "Access token lifetime (s)")}
        {numberField("identityTokenLifetimeSeconds", "Identity token lifetime (s)")}
        {numberField("refreshTokenLifetimeSeconds", "Refresh token lifetime (s)")}
      </FormDialog>

      {scopesFor && (
        <ClientScopesDialog
          applicationId={applicationId}
          client={scopesFor}
          open={scopesFor !== null}
          onOpenChange={(open) => !open && setScopesFor(null)}
        />
      )}
    </div>
  );
}
