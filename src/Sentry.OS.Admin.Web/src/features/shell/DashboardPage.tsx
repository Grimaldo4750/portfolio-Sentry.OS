import { useCurrentUser } from "@/features/auth/useCurrentUser";

/** Landing screen after sign-in. Entity screens (Users, Roles, etc.) are added by later user stories. */
export function DashboardPage() {
  const currentUser = useCurrentUser();

  return (
    <div>
      <h1 className="text-xl font-semibold">Welcome, {currentUser?.name}</h1>
      <p className="text-sm text-muted-foreground">Use the left drawer to manage your organization.</p>
    </div>
  );
}
