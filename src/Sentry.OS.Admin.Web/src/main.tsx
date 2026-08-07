import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { App } from "@/app/App";

async function enableMocking() {
  // The mock OIDC authority intercepts VITE_OIDC_AUTHORITY, which now points at the real IdP, so it
  // must stay OFF for normal dev/prod. Opt in explicitly (e.g. e2e runs) with VITE_ENABLE_MSW=true.
  if (import.meta.env.VITE_ENABLE_MSW !== "true") return;
  const { worker } = await import("../mocks/browser");
  return worker.start({ onUnhandledRequest: "bypass" });
}

void enableMocking().then(() => {
  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
});
