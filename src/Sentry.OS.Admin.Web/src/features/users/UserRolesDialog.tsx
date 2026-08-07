import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { toErrorMessage } from "@/lib/errorMessage";
import { useRoles } from "@/features/roles/api";
import { useAssignUserRole, useRemoveUserRole, useUserRoles, type User } from "@/features/users/api";

interface UserRolesDialogProps {
  organizationId: string;
  user: User;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

/** Manage a user's role assignments within the active organization. */
export function UserRolesDialog({ organizationId, user, open, onOpenChange }: UserRolesDialogProps) {
  const { t } = useTranslation();
  const [selectedRoleId, setSelectedRoleId] = useState<string>("");
  const assignments = useUserRoles(organizationId, open ? user.id : undefined);
  const roles = useRoles(open ? organizationId : undefined);
  const assign = useAssignUserRole(organizationId, user.id);
  const remove = useRemoveUserRole(organizationId, user.id);

  const assignedRoleIds = new Set((assignments.data ?? []).map((a) => a.roleId));
  const assignableRoles = (roles.data?.items ?? []).filter((r) => !assignedRoleIds.has(r.id));

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {t("users.detail.roles")} — {user.email}
          </DialogTitle>
        </DialogHeader>

        {(assign.error || remove.error) != null && (
          <FriendlyError message={toErrorMessage(assign.error ?? remove.error, t("users.detail.roleLevelBlocked"))} />
        )}

        <div className="flex flex-wrap gap-2">
          {(assignments.data ?? []).length === 0 && (
            <p className="text-sm text-muted-foreground">{t("common.table.empty")}</p>
          )}
          {(assignments.data ?? []).map((a) => (
            <Badge key={a.roleId} variant="secondary" className="gap-1.5">
              {a.roleName}
              <button
                type="button"
                aria-label={t("common.actions.delete")}
                className="ml-1 text-muted-foreground hover:text-foreground"
                onClick={() => remove.mutate(a.roleId)}
              >
                ×
              </button>
            </Badge>
          ))}
        </div>

        <div className="flex items-end gap-2">
          <Select value={selectedRoleId} onValueChange={(v) => setSelectedRoleId(v as string)}>
            <SelectTrigger className="w-full">
              <SelectValue placeholder={t("users.detail.assignRole")} />
            </SelectTrigger>
            <SelectContent>
              {assignableRoles.map((role) => (
                <SelectItem key={role.id} value={role.id}>
                  {role.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Button
            disabled={!selectedRoleId || assign.isPending}
            onClick={() => assign.mutate(selectedRoleId, { onSuccess: () => setSelectedRoleId("") })}
          >
            {t("users.detail.assignRole")}
          </Button>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            {t("common.actions.back")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
