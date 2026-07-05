import { Outlet } from "react-router-dom";
import { LeftDrawer } from "@/features/shell/LeftDrawer";
import { HeroCard } from "@/features/shell/HeroCard";

/** Permanent left-drawer layout — no top nav bar, ribbon, or dashboard header (Principle X). */
export function AppShell() {
  return (
    <div className="flex min-h-screen">
      <LeftDrawer>
        <HeroCard />
      </LeftDrawer>
      <main className="flex-1 overflow-y-auto p-6">
        <Outlet />
      </main>
    </div>
  );
}
