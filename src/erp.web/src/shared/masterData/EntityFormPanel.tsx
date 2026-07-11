import { FormEvent, ReactNode } from "react";

type EntityFormPanelProps = {
  title: string;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
  onCancel?: () => void;
  children: ReactNode;
  className?: string;
};

export function EntityFormPanel({
  title,
  onSubmit,
  onCancel,
  children,
  className = "space-y-4"
}: EntityFormPanelProps) {
  return (
    <form
      className={`rounded-md border border-slate-200 bg-white p-4 shadow-sm ${className}`}
      onSubmit={onSubmit}
    >
      <h3 className="text-lg font-semibold">{title}</h3>
      {children}
      <div className="flex gap-3">
        <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
          Guardar
        </button>
        {onCancel ? (
          <button
            type="button"
            onClick={onCancel}
            className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
          >
            Cancelar
          </button>
        ) : null}
      </div>
    </form>
  );
}
