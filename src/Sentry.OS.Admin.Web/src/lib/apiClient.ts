import axios, { type AxiosInstance } from "axios";
import { ApiError, type ApiResponse } from "@/lib/apiResponse";

let accessTokenProvider: (() => string | undefined) | undefined;
let unauthorizedHandler: (() => void) | undefined;

/** Wired by AuthProvider once a session exists (US1). */
export function registerAccessTokenProvider(provider: () => string | undefined) {
  accessTokenProvider = provider;
}

/** Wired by AuthProvider so a 401 anywhere triggers sign-out + friendly redirect (US1). */
export function registerUnauthorizedHandler(handler: () => void) {
  unauthorizedHandler = handler;
}

export const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_ADMIN_API_BASE_URL,
});

apiClient.interceptors.request.use((config) => {
  const token = accessTokenProvider?.();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 401) {
        unauthorizedHandler?.();
      }

      const envelope = error.response?.data as ApiResponse<unknown> | undefined;
      if (envelope?.responseCode) {
        return Promise.reject(new ApiError(envelope.responseCode, envelope.responseMessage));
      }

      if (!error.response) {
        return Promise.reject(new ApiError("InternalServerError", "The service is currently unreachable."));
      }
    }

    return Promise.reject(error);
  },
);

/** Unwraps the shared envelope and returns just the `data` payload. */
export async function unwrap<T>(promise: Promise<{ data: ApiResponse<T> }>): Promise<T> {
  const response = await promise;
  return response.data.data;
}
