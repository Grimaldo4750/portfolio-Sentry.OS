import "@/app/i18n";
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "react-router-dom";
import { queryClient } from "@/lib/queryClient";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { UiPreferencesProvider } from "@/app/UiPreferencesProvider";
import { router } from "@/app/routes";

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <UiPreferencesProvider>
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
      </UiPreferencesProvider>
    </QueryClientProvider>
  );
}
