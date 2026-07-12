import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  SaveSupplierAddressRequest,
  SaveSupplierContactRequest,
  SaveSupplierRequest,
  Supplier,
  supplierManagementService
} from "../masterData/supplierManagementService";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { EntityDetailPanel } from "../shared/masterData/EntityDetailPanel";
import { EntityFormPanel } from "../shared/masterData/EntityFormPanel";
import { EntityList } from "../shared/masterData/EntityList";
import { PageHeader } from "../shared/masterData/PageHeader";
import { SearchBar } from "../shared/masterData/SearchBar";
import { MasterDataListItem } from "../shared/masterData/types";

type SupplierFormState = {
  code: string;
  name: string;
  contacts: SaveSupplierContactRequest[];
  addresses: SaveSupplierAddressRequest[];
};

const emptyContact: SaveSupplierContactRequest = {
  name: "",
  email: "",
  phone: ""
};

const emptyAddress: SaveSupplierAddressRequest = {
  line1: "",
  line2: "",
  city: "",
  postalCode: "",
  country: "Angola"
};

const emptyForm: SupplierFormState = {
  code: "",
  name: "",
  contacts: [{ ...emptyContact }],
  addresses: [{ ...emptyAddress }]
};

export function SuppliersPage() {
  const { session } = useAuth();
  const [suppliers, setSuppliers] = useState<MasterDataListItem[]>([]);
  const [search, setSearch] = useState("");
  const [totalRecords, setTotalRecords] = useState(0);
  const [selectedSupplier, setSelectedSupplier] = useState<Supplier | null>(null);
  const [editingSupplier, setEditingSupplier] = useState<Supplier | null>(null);
  const [form, setForm] = useState<SupplierFormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  async function loadSuppliers(nextSearch = search) {
    if (!session) {
      return;
    }

    const result = await supplierManagementService.searchSuppliers(session, nextSearch);
    setSuppliers(result.items);
    setTotalRecords(result.totalRecords);
  }

  useEffect(() => {
    loadSuppliers().catch(() => setError("Nao foi possivel carregar os fornecedores."));
  }, [session]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      await loadSuppliers(search);
      setError(null);
    } catch {
      setError("Nao foi possivel pesquisar fornecedores.");
    }
  }

  async function handleSelectSupplier(item: MasterDataListItem) {
    if (!session) {
      return;
    }

    try {
      const supplier = await supplierManagementService.getSupplier(session, item.id);
      setSelectedSupplier(supplier);
      setError(null);
    } catch {
      setError("Nao foi possivel carregar o detalhe do fornecedor.");
    }
  }

  async function handleSaveSupplier(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!session) {
      return;
    }

    const payload: SaveSupplierRequest = {
      code: form.code,
      name: form.name,
      contacts: form.contacts,
      addresses: form.addresses
    };

    try {
      const savedSupplier = editingSupplier
        ? await supplierManagementService.updateSupplier(session, editingSupplier.id, payload)
        : await supplierManagementService.createSupplier(session, payload);

      setSelectedSupplier(savedSupplier);
      setEditingSupplier(null);
      setForm(createEmptyForm());
      await loadSuppliers();
      setError(null);
    } catch {
      setError("Nao foi possivel guardar o fornecedor.");
    }
  }

  async function handleDeactivateSupplier(supplierId: string) {
    if (!session) {
      return;
    }

    try {
      await supplierManagementService.deactivateSupplier(session, supplierId);
      setSelectedSupplier(null);
      setEditingSupplier(null);
      await loadSuppliers();
      setError(null);
    } catch {
      setError("Nao foi possivel desativar o fornecedor.");
    }
  }

  function beginEdit(supplier: Supplier) {
    setEditingSupplier(supplier);
    setForm({
      code: supplier.code,
      name: supplier.name,
      contacts: supplier.contacts.length
        ? supplier.contacts.map((contact) => ({
            name: contact.name,
            email: contact.email ?? "",
            phone: contact.phone ?? ""
          }))
        : [{ ...emptyContact }],
      addresses: supplier.addresses.length
        ? supplier.addresses.map((address) => ({
            line1: address.line1,
            line2: address.line2 ?? "",
            city: address.city,
            postalCode: address.postalCode,
            country: address.country
          }))
        : [{ ...emptyAddress }]
    });
  }

  function updateContact(index: number, field: keyof SaveSupplierContactRequest, value: string) {
    setForm((current) => ({
      ...current,
      contacts: current.contacts.map((contact, contactIndex) =>
        contactIndex === index ? { ...contact, [field]: value } : contact
      )
    }));
  }

  function updateAddress(index: number, field: keyof SaveSupplierAddressRequest, value: string) {
    setForm((current) => ({
      ...current,
      addresses: current.addresses.map((address, addressIndex) =>
        addressIndex === index ? { ...address, [field]: value } : address
      )
    }));
  }

  return (
    <section className="space-y-6">
      <PageHeader
        title="Fornecedores"
        description="Gestao de fornecedores, contactos e moradas para suportar compras e reposicao de stock."
      />

      <ErrorBanner message={error} />

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.2fr)_minmax(360px,0.9fr)]">
        <div className="space-y-4">
          <SearchBar
            value={search}
            onChange={setSearch}
            onSubmit={handleSearch}
            placeholder="Pesquisar por codigo ou nome"
          />

          <EntityList
            items={suppliers}
            totalRecords={totalRecords}
            entityLabel="fornecedores"
            emptyMessage="Nenhum fornecedor encontrado."
            onSelect={handleSelectSupplier}
          />

          {selectedSupplier ? (
            <EntityDetailPanel
              code={selectedSupplier.code}
              name={selectedSupplier.name}
              isActive={selectedSupplier.isActive}
              onEdit={() => beginEdit(selectedSupplier)}
              onDeactivate={() => handleDeactivateSupplier(selectedSupplier.id)}
            >
              <div className="grid gap-4 text-sm md:grid-cols-2">
                <div>
                  <h4 className="font-semibold text-slate-700">Contactos</h4>
                  <div className="mt-2 space-y-2">
                    {selectedSupplier.contacts.map((contact) => (
                      <p key={contact.id} className="text-slate-600">
                        <span className="font-medium text-slate-900">{contact.name}</span>
                        {contact.email ? ` - ${contact.email}` : ""}
                        {contact.phone ? ` - ${contact.phone}` : ""}
                      </p>
                    ))}
                    {selectedSupplier.contacts.length === 0 ? (
                      <p className="text-slate-500">Sem contactos.</p>
                    ) : null}
                  </div>
                </div>
                <div>
                  <h4 className="font-semibold text-slate-700">Moradas</h4>
                  <div className="mt-2 space-y-2">
                    {selectedSupplier.addresses.map((address) => (
                      <p key={address.id} className="text-slate-600">
                        <span className="font-medium text-slate-900">{address.line1}</span>
                        {address.line2 ? `, ${address.line2}` : ""}, {address.city}, {address.postalCode},{" "}
                        {address.country}
                      </p>
                    ))}
                    {selectedSupplier.addresses.length === 0 ? (
                      <p className="text-slate-500">Sem moradas.</p>
                    ) : null}
                  </div>
                </div>
              </div>
            </EntityDetailPanel>
          ) : null}
        </div>

        <EntityFormPanel
          title={editingSupplier ? "Editar fornecedor" : "Criar fornecedor"}
          onSubmit={handleSaveSupplier}
          onCancel={
            editingSupplier
              ? () => {
                  setEditingSupplier(null);
                  setForm(createEmptyForm());
                }
              : undefined
          }
          className="space-y-5"
        >
          <label className="block text-sm font-medium text-slate-700">
            Codigo
            <input
              value={form.code}
              onChange={(event) => setForm((current) => ({ ...current, code: event.target.value }))}
              required
              disabled={Boolean(editingSupplier)}
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

          <fieldset className="space-y-3">
            <legend className="text-sm font-semibold text-slate-700">Contactos</legend>
            {form.contacts.map((contact, index) => (
              <div key={index} className="space-y-2 rounded-md border border-slate-200 p-3">
                <input
                  aria-label={`Nome do contacto ${index + 1}`}
                  value={contact.name}
                  onChange={(event) => updateContact(index, "name", event.target.value)}
                  placeholder="Nome do contacto"
                  required
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                />
                <div className="grid gap-2 sm:grid-cols-2">
                  <input
                    aria-label={`Email do contacto ${index + 1}`}
                    value={contact.email}
                    onChange={(event) => updateContact(index, "email", event.target.value)}
                    placeholder="Email"
                    className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                  />
                  <input
                    aria-label={`Telefone do contacto ${index + 1}`}
                    value={contact.phone}
                    onChange={(event) => updateContact(index, "phone", event.target.value)}
                    placeholder="Telefone"
                    className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                  />
                </div>
                {form.contacts.length > 1 ? (
                  <button
                    type="button"
                    onClick={() =>
                      setForm((current) => ({
                        ...current,
                        contacts: current.contacts.filter((_, contactIndex) => contactIndex !== index)
                      }))
                    }
                    className="text-sm font-medium text-red-700"
                  >
                    Remover contacto
                  </button>
                ) : null}
              </div>
            ))}
            <button
              type="button"
              onClick={() =>
                setForm((current) => ({ ...current, contacts: [...current.contacts, { ...emptyContact }] }))
              }
              className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
            >
              Adicionar contacto
            </button>
          </fieldset>

          <fieldset className="space-y-3">
            <legend className="text-sm font-semibold text-slate-700">Moradas</legend>
            {form.addresses.map((address, index) => (
              <div key={index} className="space-y-2 rounded-md border border-slate-200 p-3">
                <input
                  aria-label={`Linha 1 da morada ${index + 1}`}
                  value={address.line1}
                  onChange={(event) => updateAddress(index, "line1", event.target.value)}
                  placeholder="Linha 1"
                  required
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                />
                <input
                  aria-label={`Linha 2 da morada ${index + 1}`}
                  value={address.line2}
                  onChange={(event) => updateAddress(index, "line2", event.target.value)}
                  placeholder="Linha 2"
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                />
                <div className="grid gap-2 sm:grid-cols-3">
                  <input
                    aria-label={`Cidade da morada ${index + 1}`}
                    value={address.city}
                    onChange={(event) => updateAddress(index, "city", event.target.value)}
                    placeholder="Cidade"
                    required
                    className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                  />
                  <input
                    aria-label={`Codigo postal da morada ${index + 1}`}
                    value={address.postalCode}
                    onChange={(event) => updateAddress(index, "postalCode", event.target.value)}
                    placeholder="Codigo postal"
                    required
                    className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                  />
                  <input
                    aria-label={`Pais da morada ${index + 1}`}
                    value={address.country}
                    onChange={(event) => updateAddress(index, "country", event.target.value)}
                    placeholder="Pais"
                    required
                    className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                  />
                </div>
                {form.addresses.length > 1 ? (
                  <button
                    type="button"
                    onClick={() =>
                      setForm((current) => ({
                        ...current,
                        addresses: current.addresses.filter((_, addressIndex) => addressIndex !== index)
                      }))
                    }
                    className="text-sm font-medium text-red-700"
                  >
                    Remover morada
                  </button>
                ) : null}
              </div>
            ))}
            <button
              type="button"
              onClick={() =>
                setForm((current) => ({ ...current, addresses: [...current.addresses, { ...emptyAddress }] }))
              }
              className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
            >
              Adicionar morada
            </button>
          </fieldset>
        </EntityFormPanel>
      </div>
    </section>
  );
}

function createEmptyForm(): SupplierFormState {
  return {
    code: "",
    name: "",
    contacts: [{ ...emptyContact }],
    addresses: [{ ...emptyAddress }]
  };
}
