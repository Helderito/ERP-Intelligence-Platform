import { ReactNode } from "react";

type EntityDetailPanelProps = {
  code: string;
  name: string;
  isActive: boolean;
  onEdit: () => void;
  onDeactivate: () => void;
  children?: ReactNode;
};

export function EntityDetailPanel({
  code,
  name,
  isActive,
  onEdit,
  onDeactivate,
  children
}: EntityDetailPanelProps) {
  return (
    <article className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="font-mono text-sm font-semibold text-brand-700">{code}</p>
          <h3 className="mt-1 text-xl font-semibold">{name}</h3>
        </div>
        <span className="rounded-md bg-slate-100 px-3 py-1 text-sm font-medium text-slate-700">
          {isActive ? "Ativo" : "Inativo"}
        </span>
      </div>
      {children ? <div className="mt-4">{children}</div> : null}
      <div className="mt-4 flex gap-3">
        <button
          type="button"
          onClick={onEdit}
          className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
        >
          Editar
        </button>
        <button
          type="button"
          onClick={onDeactivate}
          className="rounded-md border border-red-200 px-3 py-2 text-sm font-medium text-red-700"
        >
          Desativar
        </button>
      </div>
    </article>
  );
}
