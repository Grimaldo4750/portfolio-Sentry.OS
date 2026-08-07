import type { ReactNode } from "react";
import { Link } from "react-router-dom";
import { ArrowLeft } from "lucide-react";

interface ListPageHeaderProps {
  title: string;
  action?: ReactNode;
  back?: { to: string; label: string };
}

/** Standard header for every management list screen: optional back link, title, and a primary action. */
export function ListPageHeader({ title, action, back }: ListPageHeaderProps) {
  return (
    <div className="mb-4 flex flex-col gap-2">
      {back && (
        <Link to={back.to} className="flex w-fit items-center gap-1 text-xs text-muted-foreground hover:text-foreground">
          <ArrowLeft className="size-3" aria-hidden="true" />
          {back.label}
        </Link>
      )}
      <div className="flex items-center justify-between gap-2">
        <h1 className="text-xl font-semibold">{title}</h1>
        {action}
      </div>
    </div>
  );
}
