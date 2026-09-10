import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../../auth/useAuth";
import {
  ManagedReferenceDataService,
  SaveReferenceDataRequest
} from "../../masterData/managedReferenceDataService";
import { EntityDetailPanel } from "./EntityDetailPanel";
import { EntityFormPanel } from "./EntityFormPanel";
import { EntityList } from "./EntityList";
import { ErrorBanner } from "./ErrorBanner";
import { PageHeader } from "./PageHeader";
import { SearchBar } from "./SearchBar";
import { MasterDataListItem } from "./types";

type Props = {
  title: string;
  description: string;
  entityLabel: string;
  formEntityName: string;
  emptyMessage: string;
  loadError: string;
  saveError: string;
  deactivateError: string;
  service: ManagedReferenceDataService;
};

const emptyForm = { code: "", name: "" };

export function ManagedReferenceDataPage(props: Props) {
  const { session } = useAuth();
  const [items, setItems] = useState<MasterDataListItem[]>([]);
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<MasterDataListItem | null>(null);
  const [editing, setEditing] = useState<MasterDataListItem | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  async function loadData(nextSearch = search) {
    if (!session) return;
    setItems(await props.service.list(session, nextSearch));
  }

  useEffect(() => {
    loadData().catch(() => setError(props.loadError));
  }, [session?.accessToken]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      await loadData(search);
      setError(null);
    } catch {
      setError(props.loadError);
    }
  }

  async function handleSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session) return;

    const payload: SaveReferenceDataRequest = { code: form.code, name: form.name };
    try {
      const saved = editing
        ? await props.service.update(session, editing.id, payload)
        : await props.service.create(session, payload);
      setSelected(saved);
      setEditing(null);
      setForm(emptyForm);
      await loadData();
      setError(null);
    } catch {
      setError(props.saveError);
    }
  }

  async function handleDeactivate(id: string) {
    if (!session) return;
    try {
      await props.service.deactivate(session, id);
      setSelected(null);
      setEditing(null);
      await loadData();
      setError(null);
    } catch {
      setError(props.deactivateError);
    }
  }

  function beginEdit(item: MasterDataListItem) {
    setEditing(item);
    setForm({ code: item.code, name: item.name });
  }

  return (
    <section className="space-y-6">
      <PageHeader title={props.title} description={props.description} />
      <ErrorBanner message={error} />
      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(320px,0.8fr)]">
        <div className="space-y-4">
          <SearchBar value={search} onChange={setSearch} onSubmit={handleSearch} placeholder="Pesquisar por código ou nome" />
          <EntityList
            items={items}
            totalRecords={items.length}
            entityLabel={props.entityLabel}
            emptyMessage={props.emptyMessage}
            onSelect={setSelected}
          />
          {selected ? (
            <EntityDetailPanel
              code={selected.code}
              name={selected.name}
              isActive={selected.isActive}
              onEdit={() => beginEdit(selected)}
              onDeactivate={() => handleDeactivate(selected.id)}
            />
          ) : null}
        </div>
        <EntityFormPanel
          title={editing ? `Editar ${props.formEntityName}` : `Criar ${props.formEntityName}`}
          onSubmit={handleSave}
          onCancel={editing ? () => { setEditing(null); setForm(emptyForm); } : undefined}
        >
          <label className="block text-sm font-medium text-slate-700">
            Código
            <input
              value={form.code}
              onChange={(event) => setForm((current) => ({ ...current, code: event.target.value }))}
              required
              disabled={Boolean(editing)}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm disabled:bg-slate-100"
            />
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Nome
            <input
              value={form.name}
              onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            />
          </label>
        </EntityFormPanel>
      </div>
    </section>
  );
}
