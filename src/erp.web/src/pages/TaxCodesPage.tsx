import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  SaveTaxCodeRequest,
  TaxCode,
  taxCodeManagementService
} from "../masterData/taxCodeManagementService";
import { EntityDetailPanel } from "../shared/masterData/EntityDetailPanel";
import { EntityFormPanel } from "../shared/masterData/EntityFormPanel";
import { EntityList } from "../shared/masterData/EntityList";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { PageHeader } from "../shared/masterData/PageHeader";
import { SearchBar } from "../shared/masterData/SearchBar";

const emptyForm = { code: "", name: "", rate: "0" };

export function TaxCodesPage() {
  const { session } = useAuth();
  const [taxCodes, setTaxCodes] = useState<TaxCode[]>([]);
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<TaxCode | null>(null);
  const [editing, setEditing] = useState<TaxCode | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState<string | null>(null);

  async function loadData(nextSearch = search) {
    if (!session) return;
    setTaxCodes(await taxCodeManagementService.list(session, nextSearch));
  }

  useEffect(() => {
    loadData().catch(() => setError("Não foi possível carregar os códigos de imposto."));
  }, [session?.accessToken]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      await loadData(search);
      setError(null);
    } catch {
      setError("Não foi possível pesquisar códigos de imposto.");
    }
  }

  async function handleSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session) return;

    const payload: SaveTaxCodeRequest = {
      code: form.code,
      name: form.name,
      rate: Number(form.rate)
    };
    try {
      const saved = editing
        ? await taxCodeManagementService.update(session, editing.id, payload)
        : await taxCodeManagementService.create(session, payload);
      setSelected(saved);
      setEditing(null);
      setForm(emptyForm);
      await loadData();
      setError(null);
    } catch {
      setError("Não foi possível guardar o código de imposto.");
    }
  }

  async function handleDeactivate(id: string) {
    if (!session) return;
    try {
      await taxCodeManagementService.deactivate(session, id);
      setSelected(null);
      setEditing(null);
      await loadData();
      setError(null);
    } catch {
      setError("Não foi possível desativar o código de imposto.");
    }
  }

  function beginEdit(taxCode: TaxCode) {
    setEditing(taxCode);
    setForm({ code: taxCode.code, name: taxCode.name, rate: taxCode.rate.toString() });
  }

  return (
    <section className="space-y-6">
      <PageHeader
        title="Códigos de Imposto"
        description="Gestão das taxas de referência, sem cálculos ou regras fiscais."
      />
      <ErrorBanner message={error} />
      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(320px,0.8fr)]">
        <div className="space-y-4">
          <SearchBar value={search} onChange={setSearch} onSubmit={handleSearch} placeholder="Pesquisar por código ou nome" />
          <EntityList
            items={taxCodes}
            totalRecords={taxCodes.length}
            entityLabel="códigos"
            emptyMessage="Nenhum código de imposto encontrado."
            onSelect={(item) => setSelected(taxCodes.find((taxCode) => taxCode.id === item.id) ?? null)}
          />
          {selected ? (
            <EntityDetailPanel
              code={selected.code}
              name={selected.name}
              isActive={selected.isActive}
              onEdit={() => beginEdit(selected)}
              onDeactivate={() => handleDeactivate(selected.id)}
            >
              <dl className="text-sm">
                <dt className="font-medium text-slate-500">Taxa</dt>
                <dd className="mt-1 text-slate-900">{selected.rate}%</dd>
              </dl>
            </EntityDetailPanel>
          ) : null}
        </div>
        <EntityFormPanel
          title={editing ? "Editar código de imposto" : "Criar código de imposto"}
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
          <label className="block text-sm font-medium text-slate-700">
            Taxa (%)
            <input
              type="number"
              min="0"
              max="100"
              step="0.01"
              value={form.rate}
              onChange={(event) => setForm((current) => ({ ...current, rate: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            />
          </label>
        </EntityFormPanel>
      </div>
    </section>
  );
}
