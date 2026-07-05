import { AlertTriangle } from "lucide-react";
import { cn } from "@/lib/utils";

interface FriendlyErrorProps {
  message: string;
  className?: string;
}

/** A friendly, localized error surface — used for forbidden screens, connectivity errors, and expired sessions. */
export function FriendlyError({ message, className }: FriendlyErrorProps) {
  return (
    <div
      role="alert"
      className={cn(
        "flex items-center gap-2 rounded-md border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive",
        className,
      )}
    >
      <AlertTriangle className="size-4 shrink-0" aria-hidden="true" />
      <span>{message}</span>
    </div>
  );
}
