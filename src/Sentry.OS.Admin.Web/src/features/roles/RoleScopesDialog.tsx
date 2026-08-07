import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { toErrorMessage } from "@/lib/errorMessage";
import { useOrganizationScopes } from "@/features/applications/api";
import { useAttachRoleScope, useDetachRoleScope, useRoles, type Role } from "@/features/roles/api";

interface RoleScopesDialogProps {
  organizationId: string;
  role: Role;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

/** Attach/detach API scopes to a role. Scope ids are resolved from the org-wide scope catalog. */
export function RoleScopesDialog({ organizationId, role, open, onOpenChange }: RoleScopesDialogProps) {
  const { t } = useTranslation();
  const [selectedScopeId, setSelectedScopeId] = useState<string>("");
  const { scopes } = useOrganizationScopes(open ? organizationId : undefined);
  const roles = useRoles(open ? organizationId : undefined);
  const attach = useAttachRoleScope(organizationId);
  const detach = useDetachRoleScope(organizationId);

  // Read the live role from the cached list so attach/detach reflect immediately (the passed role
  // is a snapshot from when the dialog opened).
  const live = roles.data?.items.find((r) => r.id === role.id) ?? role;
  const attachedNames = new Set(live.scopeNames);
  const attachable = scopes.filter((s) => !attachedNames.has(s.name));
  const scopeIdByName = new Map(scopes.map((s) => [s.name, s.id]));

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {t("roles.detail.scopes")} — {role.name}
          </DialogTitle>
        </DialogHeader>

        {(attach.error || detach.error) != null && (
          <FriendlyError message={toErrorMessage(attach.error ?? detach.error, t("common.errors.unexpected"))} />
        )}

        <div className="flex flex-wrap gap-2">
          {live.scopeNames.length === 0 && <p className="text-sm text-muted-foreground">{t("common.table.empty")}</p>}
          {live.scopeNames.map((name) => {
            const scopeId = scopeIdByName.get(name);
            return (
              <Badge key={name} variant="secondary" className="gap-1.5">
                {name}
                {scopeId && (
                  <button
                    type="button"
                    aria-label={t("common.actions.delete")}
                    className="ml-1 text-muted-foreground hover:text-foreground"
                    onClick={() => detach.mutate({ roleId: role.id, scopeId })}
                  >
                    ×
                  </button>
                )}
              </Badge>
            );
          })}
        </div>

        <div className="flex items-end gap-2">
          <Select value={selectedScopeId} onValueChange={(v) => setSelectedScopeId(v as string)}>
            <SelectTrigger className="w-full">
              <SelectValue placeholder={t("roles.detail.attachScope")} />
            </SelectTrigger>
            <SelectContent>
              {attachable.map((scope) => (
                <SelectItem key={scope.id} value={scope.id}>
                  {scope.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Button
            disabled={!selectedScopeId || attach.isPending}
            onClick={() => attach.mutate({ roleId: role.id, scopeId: selectedScopeId }, { onSuccess: () => setSelectedScopeId("") })}
          >
            {t("roles.detail.attachScope")}
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
