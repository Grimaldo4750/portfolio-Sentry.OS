import { NavLink } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { ReactNode } from "react";
import { cn } from "@/lib/utils";
import { useCurrentUser } from "@/features/auth/useCurrentUser";

interface NavEntry {
  to: string;
  labelKey: string;
  globalAdministratorOnly?: boolean;
}

const NAV_ENTRIES: NavEntry[] = [
  { to: "/users", labelKey: "shell.nav.users" },
  { to: "/roles", labelKey: "shell.nav.roles" },
  { to: "/applications", labelKey: "shell.nav.applications" },
  { to: "/organizations", labelKey: "shell.nav.organizations", globalAdministratorOnly: true },
  { to: "/audit-log", labelKey: "shell.nav.auditLog" },
];

/** The permanent left drawer's nav sidebar — no top nav/ribbon/dashboard header (Principle X, NON-NEGOTIABLE). */
export function LeftDrawer({ children }: { children: ReactNode }) {
  const { t } = useTranslation();
  const currentUser = useCurrentUser();

  const entries = NAV_ENTRIES.filter(
    (entry) => !entry.globalAdministratorOnly || currentUser?.isGlobalAdministrator,
  );

  return (
    <nav className="flex w-64 shrink-0 flex-col gap-1 border-r border-border bg-card p-4">
      {children}
      <ul className="mt-4 flex flex-col gap-1">
        {entries.map((entry) => (
          <li key={entry.to}>
            <NavLink
              to={entry.to}
              className={({ isActive }) =>
                cn(
                  "block rounded-md px-3 py-2 text-sm font-medium hover:bg-muted",
                  isActive && "bg-muted text-foreground",
                )
              }
            >
              {t(entry.labelKey)}
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  );
}
