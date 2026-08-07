import { useTranslation } from "react-i18next";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { useCurrentUser } from "@/features/auth/useCurrentUser";
import { useActiveOrganization } from "@/features/shell/ActiveOrganizationProvider";
import { useOrganizations } from "@/features/organizations/api";

/**
 * Hero Card organization switcher. Global administrators pick any organization to scope every
 * management screen; other users stay pinned to their home organization (the org list endpoint is
 * global-admin only, so no switcher is shown for them).
 */
export function OrganizationSwitcher() {
  const { t } = useTranslation();
  const currentUser = useCurrentUser();
  const { activeOrganizationId, setActiveOrganizationId } = useActiveOrganization();
  const isGlobalAdmin = currentUser?.isGlobalAdministrator ?? false;
  const { data } = useOrganizations(1, 200, isGlobalAdmin);

  if (!isGlobalAdmin) {
    return null;
  }

  const activeName =
    data?.items.find((org) => org.id === activeOrganizationId)?.displayName ?? "";

  return (
    <div className="flex flex-col gap-1.5">
      <Label htmlFor="organization-switcher" className="text-xs text-muted-foreground">
        {t("shell.organizationSwitcher.label")}
      </Label>
      <Select
        value={activeOrganizationId ?? ""}
        onValueChange={(value) => setActiveOrganizationId(value as string)}
      >
        <SelectTrigger id="organization-switcher" className="w-full">
          <SelectValue>{activeName}</SelectValue>
        </SelectTrigger>
        <SelectContent>
          {(data?.items ?? []).map((org) => (
            <SelectItem key={org.id} value={org.id}>
              {org.displayName}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
