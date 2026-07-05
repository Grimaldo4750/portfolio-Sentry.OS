import { setupServer } from "msw/node";
import { oidcHandlers } from "../mocks/oidcHandlers";

export const server = setupServer(...oidcHandlers);
