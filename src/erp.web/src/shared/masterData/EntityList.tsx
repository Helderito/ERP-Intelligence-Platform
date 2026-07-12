import { MasterDataListItem } from "./types";
import { StatusLabel } from "./StatusLabel";

type EntityListProps = {
  items: MasterDataListItem[];
  totalRecords: number;
  entityLabel: string;
  emptyMessage: string;
  onSelect: (item: MasterDataListItem) => void;
};

export function EntityList({ items, totalRecords, entityLabel, emptyMessage, onSelect }: EntityListProps) {
  return (
    <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
      <div className="border-b border-slate-200 px-4 py-3 text-sm text-slate-600">
        {totalRecords} {entityLabel} encontrados
      </div>
      <div className="divide-y divide-slate-200">
        {items.map((item) => (
          <button
            key={item.id}
            type="button"
            onClick={() => onSelect(item)}
            className="grid w-full grid-cols-[120px_minmax(0,1fr)_100px] gap-4 px-4 py-3 text-left text-sm transition hover:bg-slate-50"
          >
            <span className="font-mono font-semibold text-slate-900">{item.code}</span>
            <span className="truncate font-medium text-slate-800">{item.name}</span>
            <StatusLabel isActive={item.isActive} />
          </button>
        ))}
        {items.length === 0 ? <p className="px-4 py-6 text-sm text-slate-500">{emptyMessage}</p> : null}
      </div>
    </div>
  );
}
