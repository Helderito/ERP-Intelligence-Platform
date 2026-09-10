import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  SaveWarehouseRequest,
  Warehouse,
  WarehouseType,
  warehouseManagementService
} from "../masterData/warehouseManagementService";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { EntityDetailPanel } from "../shared/masterData/EntityDetailPanel";
import { EntityFormPanel } from "../shared/masterData/EntityFormPanel";
import { EntityList } from "../shared/masterData/EntityList";
import { PageHeader } from "../shared/masterData/PageHeader";
import { SearchBar } from "../shared/masterData/SearchBar";
import { MasterDataListItem } from "../shared/masterData/types";

type WarehouseFormState = {
  code: string;
  name: string;
  warehouseTypeId: string;
};

const emptyForm: WarehouseFormState = { code: "", name: "", warehouseTypeId: "" };

export function WarehousesPage() {
  const { session } = useAuth();
  const [warehouses, setWarehouses] = useState<MasterDataListItem[]>([]);
  const [warehouseTypes, setWarehouseTypes] = useState<WarehouseType[]>([]);
  const [search, setSearch] = useState("");
  const [totalRecords, setTotalRecords] = useState(0);
  const [selectedWarehouse, setSelectedWarehouse] = useState<Warehouse | null>(null);
  const [editingWarehouse, setEditingWarehouse] = useState<Warehouse | null>(null);
  const [form, setForm] = useState<WarehouseFormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  async function loadData(nextSearch = search) {
    if (!session) {
      return;
    }

    const [warehouseResult, nextWarehouseTypes] = await Promise.all([
      warehouseManagementService.searchWarehouses(session, nextSearch),
      warehouseManagementService.getWarehouseTypes(session)
    ]);

    setWarehouses(warehouseResult.items);
    setTotalRecords(warehouseResult.totalRecords);
    setWarehouseTypes(nextWarehouseTypes);
    setForm((current) => current.warehouseTypeId
      ? current
      : { ...current, warehouseTypeId: nextWarehouseTypes[0]?.id || "" });
  }

  useEffect(() => {
    loadData().catch(() => setError("Não foi possível carregar os armazéns."));
  }, [session]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      await loadData(search);
      setError(null);
    } catch {
      setError("Não foi possível pesquisar armazéns.");
    }
  }

  async function handleSelectWarehouse(item: MasterDataListItem) {
    if (!session) {
      return;
    }

    try {
      setSelectedWarehouse(await warehouseManagementService.getWarehouse(session, item.id));
      setError(null);
    } catch {
      setError("Não foi possível carregar o detalhe do armazém.");
    }
  }

  async function handleSaveWarehouse(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session) {
      return;
    }

    const payload: SaveWarehouseRequest = {
      code: form.code,
      name: form.name,
      warehouseTypeId: form.warehouseTypeId
    };

    try {
      const savedWarehouse = editingWarehouse
        ? await warehouseManagementService.updateWarehouse(session, editingWarehouse.id, payload)
        : await warehouseManagementService.createWarehouse(session, payload);

      setSelectedWarehouse(savedWarehouse);
      setEditingWarehouse(null);
      setForm({ ...emptyForm, warehouseTypeId: warehouseTypes[0]?.id ?? "" });
      await loadData();
      setError(null);
    } catch {
      setError("Não foi possível guardar o armazém.");
    }
  }

  async function handleDeactivateWarehouse(warehouseId: string) {
    if (!session) {
      return;
    }

    try {
      await warehouseManagementService.deactivateWarehouse(session, warehouseId);
      setSelectedWarehouse(null);
      setEditingWarehouse(null);
      await loadData();
      setError(null);
    } catch {
      setError("Não foi possível desativar o armazém.");
    }
  }

  function beginEdit(warehouse: Warehouse) {
    setEditingWarehouse(warehouse);
    setForm({
      code: warehouse.code,
      name: warehouse.name,
      warehouseTypeId: warehouse.warehouseTypeId
    });
  }

  return (
    <section className="space-y-6">
      <PageHeader
        title="Armazéns"
        description="Gestão dos armazéns organizacionais e respetivos tipos."
      />

      <ErrorBanner message={error} />

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(320px,0.8fr)]">
        <div className="space-y-4">
          <SearchBar
            value={search}
            onChange={setSearch}
            onSubmit={handleSearch}
            placeholder="Pesquisar por código ou nome"
          />

          <EntityList
            items={warehouses}
            totalRecords={totalRecords}
            entityLabel="armazéns"
            emptyMessage="Nenhum armazém encontrado."
            onSelect={handleSelectWarehouse}
          />

          {selectedWarehouse ? (
            <EntityDetailPanel
              code={selectedWarehouse.code}
              name={selectedWarehouse.name}
              isActive={selectedWarehouse.isActive}
              onEdit={() => beginEdit(selectedWarehouse)}
              onDeactivate={() => handleDeactivateWarehouse(selectedWarehouse.id)}
            >
              <dl className="text-sm">
                <dt className="font-medium text-slate-500">Tipo de armazém</dt>
                <dd className="mt-1 text-slate-900">{selectedWarehouse.warehouseTypeName}</dd>
              </dl>
            </EntityDetailPanel>
          ) : null}
        </div>

        <EntityFormPanel
          title={editingWarehouse ? "Editar armazém" : "Criar armazém"}
          onSubmit={handleSaveWarehouse}
          onCancel={editingWarehouse ? () => {
            setEditingWarehouse(null);
            setForm({ ...emptyForm, warehouseTypeId: warehouseTypes[0]?.id ?? "" });
          } : undefined}
        >
          <label className="block text-sm font-medium text-slate-700">
            Código
            <input
              value={form.code}
              onChange={(event) => setForm((current) => ({ ...current, code: event.target.value }))}
              required
              disabled={Boolean(editingWarehouse)}
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
            Tipo de armazém
            <select
              value={form.warehouseTypeId}
              onChange={(event) => setForm((current) => ({ ...current, warehouseTypeId: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            >
              <option value="">Selecionar tipo</option>
              {warehouseTypes.map((warehouseType) => (
                <option key={warehouseType.id} value={warehouseType.id}>{warehouseType.name}</option>
              ))}
            </select>
          </label>
        </EntityFormPanel>
      </div>
    </section>
  );
}
