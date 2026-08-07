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
import { UserRolesDialog } from "@/features/users/UserRolesDialog";
import {
  useCreateUser,
  useDeactivateUser,
  useUpdateUser,
  useUsers,
  type User,
} from "@/features/users/api";

type DialogState = { mode: "create" } | { mode: "edit"; user: User } | null;

const EMPTY = { email: "", userName: "", firstName: "", lastName: "" };

/** Users management for the active organization, including role assignments. */
export function UsersPage() {
  const { t } = useTranslation();
  const { activeOrganizationId } = useActiveOrganization();
  const [page, setPage] = useState(1);
  const [dialog, setDialog] = useState<DialogState>(null);
  const [rolesFor, setRolesFor] = useState<User | null>(null);
  const [form, setForm] = useState(EMPTY);

  const { data, isError } = useUsers(activeOrganizationId, page);
  const create = useCreateUser(activeOrganizationId ?? "");
  const update = useUpdateUser(activeOrganizationId ?? "");
  const deactivate = useDeactivateUser(activeOrganizationId ?? "");

  if (!activeOrganizationId) {
    return <FriendlyError message={t("common.errors.unexpected")} />;
  }

  const openCreate = () => {
    setForm(EMPTY);
    create.reset();
    setDialog({ mode: "create" });
  };

  const openEdit = (user: User) => {
    setForm({ email: user.email, userName: user.userName, firstName: user.firstName ?? "", lastName: user.lastName ?? "" });
    update.reset();
    setDialog({ mode: "edit", user });
  };

  const submit = () => {
    if (dialog?.mode === "create") {
      create.mutate(
        { email: form.email, userName: form.userName, firstName: form.firstName || null, lastName: form.lastName || null },
        { onSuccess: () => setDialog(null) },
      );
    } else if (dialog?.mode === "edit") {
      update.mutate(
        { id: dialog.user.id, body: { firstName: form.firstName || null, lastName: form.lastName || null, profilePictureUrl: dialog.user.profilePictureUrl } },
        { onSuccess: () => setDialog(null) },
      );
    }
  };

  const fullName = (u: User) => [u.firstName, u.lastName].filter(Boolean).join(" ") || u.userName;

  return (
    <div>
      <ListPageHeader
        title={t("users.list.title")}
        action={<Button onClick={openCreate}>{t("users.list.create")}</Button>}
      />

      {isError && <FriendlyError message={t("common.errors.unexpected")} className="mb-4" />}

      <PagedTable
        items={data?.items ?? []}
        page={page}
        pageSize={data?.pageSize ?? 50}
        totalCount={data?.totalCount ?? 0}
        onPageChange={setPage}
        rowKey={(u) => u.id}
        columns={[
          { key: "name", header: t("users.detail.nameLabel") },
          { key: "email", header: t("users.detail.emailLabel") },
          { key: "status", header: "Status" },
          { key: "actions", header: "" },
        ]}
        renderRow={(user) => (
          <TableRow>
            <TableCell className="font-medium">{fullName(user)}</TableCell>
            <TableCell className="text-muted-foreground">{user.email}</TableCell>
            <TableCell>
              <Badge variant={user.isDisabled ? "outline" : "secondary"}>
                {t(user.isDisabled ? "common.status.inactive" : "common.status.active")}
              </Badge>
            </TableCell>
            <TableCell className="flex justify-end gap-2">
              <Button variant="outline" size="sm" onClick={() => setRolesFor(user)}>
                {t("users.detail.roles")}
              </Button>
              <Button variant="outline" size="sm" onClick={() => openEdit(user)}>
                {t("common.actions.edit")}
              </Button>
              {!user.isDisabled && (
                <ConfirmDeactivateDialog
                  trigger={
                    <Button variant="outline" size="sm">
                      {t("common.actions.deactivate")}
                    </Button>
                  }
                  title={t("common.actions.deactivate")}
                  description={user.email}
                  isPending={deactivate.isPending}
                  onConfirm={() => deactivate.mutate(user.id)}
                />
              )}
            </TableCell>
          </TableRow>
        )}
      />

      <FormDialog
        open={dialog !== null}
        onOpenChange={(open) => !open && setDialog(null)}
        title={dialog?.mode === "edit" ? t("common.actions.edit") : t("users.list.create")}
        onSubmit={submit}
        isPending={create.isPending || update.isPending}
        error={dialog?.mode === "edit" ? update.error : create.error}
      >
        <Field label={t("users.detail.emailLabel")} htmlFor="user-email">
          <Input
            id="user-email"
            type="email"
            value={form.email}
            disabled={dialog?.mode === "edit"}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            required
          />
        </Field>
        <Field label="Username" htmlFor="user-username">
          <Input
            id="user-username"
            value={form.userName}
            disabled={dialog?.mode === "edit"}
            onChange={(e) => setForm({ ...form, userName: e.target.value })}
            required
          />
        </Field>
        <Field label="First name" htmlFor="user-first">
          <Input id="user-first" value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} />
        </Field>
        <Field label="Last name" htmlFor="user-last">
          <Input id="user-last" value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} />
        </Field>
      </FormDialog>

      {rolesFor && (
        <UserRolesDialog
          organizationId={activeOrganizationId}
          user={rolesFor}
          open={rolesFor !== null}
          onOpenChange={(open) => !open && setRolesFor(null)}
        />
      )}
    </div>
  );
}
