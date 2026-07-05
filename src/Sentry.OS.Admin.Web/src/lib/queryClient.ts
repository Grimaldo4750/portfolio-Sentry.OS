import { QueryClient } from "@tanstack/react-query";
import { ApiError } from "@/lib/apiResponse";

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: (failureCount, error) => {
        if (error instanceof ApiError) {
          // Validation/auth/not-found/conflict errors will never succeed on retry.
          return false;
        }
        return failureCount < 2;
      },
    },
    mutations: {
      retry: false,
    },
  },
});
