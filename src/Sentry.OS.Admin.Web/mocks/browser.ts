import { setupWorker } from "msw/browser";
import { oidcHandlers } from "./oidcHandlers";

export const worker = setupWorker(...oidcHandlers);
