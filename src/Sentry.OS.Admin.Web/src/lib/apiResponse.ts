export type ResponseCode =
  | "Success"
  | "ValidationError"
  | "Unauthorized"
  | "Forbidden"
  | "NotFound"
  | "Conflict"
  | "InternalServerError";

export interface ApiResponse<T> {
  responseCode: ResponseCode;
  responseMessage: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export class ApiError extends Error {
  readonly responseCode: ResponseCode;

  constructor(responseCode: ResponseCode, message: string) {
    super(message);
    this.name = "ApiError";
    this.responseCode = responseCode;
  }
}
