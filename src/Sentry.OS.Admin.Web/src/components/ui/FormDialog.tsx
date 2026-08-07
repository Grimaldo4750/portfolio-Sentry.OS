import type { FormEvent, ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { FriendlyError } from "@/components/ui/FriendlyError";
import { toErrorMessage } from "@/lib/errorMessage";

interface FormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  onSubmit: () => void;
  isPending?: boolean;
  error?: unknown;
  submitLabel?: string;
  children: ReactNode;
}

/** Shared controlled create/edit dialog: renders a form with fields, an error surface, and footer actions. */
export function FormDialog({
  open,
  onOpenChange,
  title,
  onSubmit,
  isPending,
  error,
  submitLabel,
  children,
}: FormDialogProps) {
  const { t } = useTranslation();

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    onSubmit();
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
          </DialogHeader>

          {error != null && <FriendlyError message={toErrorMessage(error, t("common.errors.unexpected"))} />}

          <div className="flex flex-col gap-3">{children}</div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              {t("common.actions.cancel")}
            </Button>
            <Button type="submit" disabled={isPending}>
              {submitLabel ?? t("common.actions.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
