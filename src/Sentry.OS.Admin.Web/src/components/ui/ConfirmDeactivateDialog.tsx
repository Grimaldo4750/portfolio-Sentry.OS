import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import type { ReactElement } from "react";

interface ConfirmDeactivateDialogProps {
  trigger: ReactElement;
  title: string;
  description: string;
  onConfirm: () => void;
  isPending?: boolean;
}

/** Shared confirmation dialog used by every entity's deactivate action. */
export function ConfirmDeactivateDialog({
  trigger,
  title,
  description,
  onConfirm,
  isPending,
}: ConfirmDeactivateDialogProps) {
  const { t } = useTranslation();

  return (
    <Dialog>
      <DialogTrigger render={trigger} />
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogTrigger render={<Button variant="outline">{t("common.actions.cancel")}</Button>} />
          <Button variant="destructive" onClick={onConfirm} disabled={isPending}>
            {t("common.actions.confirm")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
