import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { toErrorMessage } from "@/lib/errorMessage";
import { useActiveOrganization } from "@/features/shell/ActiveOrganizationProvider";
import { useOrganizationScopes, useSetClientScopes, type Client } from "@/features/applications/api";

interface ClientScopesDialogProps {
  applicationId: string;
  client: Client;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

/** Set the exact set of API scopes a client is allowed to request (full replacement). */
export function ClientScopesDialog({ applicationId, client, open, onOpenChange }: ClientScopesDialogProps) {
  const { t } = useTranslation();
  const { activeOrganizationId } = useActiveOrganization();
  const { scopes } = useOrganizationScopes(open ? activeOrganizationId : undefined);
  const setScopes = useSetClientScopes(applicationId);

  const [selected, setSelected] = useState<Set<string>>(new Set());

  // Seed the checkbox state from the client's current allowed scopes once the catalog is available.
  useEffect(() => {
    if (open) {
      const allowed = new Set(client.allowedScopeNames);
      setSelected(new Set(scopes.filter((s) => allowed.has(s.name)).map((s) => s.id)));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, scopes.length, client.id]);

  const toggle = (scopeId: string) => {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(scopeId)) next.delete(scopeId);
      else next.add(scopeId);
      return next;
    });
  };

  const save = () => {
    setScopes.mutate(
      { clientId: client.id, scopeIds: [...selected] },
      { onSuccess: () => onOpenChange(false) },
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {t("clients.detail.allowedScopes")} — {client.displayName}
          </DialogTitle>
        </DialogHeader>

        {setScopes.error != null && <FriendlyError message={toErrorMessage(setScopes.error, t("common.errors.unexpected"))} />}

        <div className="flex max-h-72 flex-col gap-2 overflow-y-auto">
          {scopes.length === 0 && <p className="text-sm text-muted-foreground">{t("common.table.empty")}</p>}
          {scopes.map((scope) => (
            <label key={scope.id} className="flex items-center gap-2 text-sm">
              <input type="checkbox" checked={selected.has(scope.id)} onChange={() => toggle(scope.id)} />
              <span className="font-medium">{scope.name}</span>
              <span className="text-muted-foreground">— {scope.displayName}</span>
            </label>
          ))}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            {t("common.actions.cancel")}
          </Button>
          <Button onClick={save} disabled={setScopes.isPending}>
            {t("common.actions.save")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
