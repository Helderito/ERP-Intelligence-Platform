import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  Customer,
  customerManagementService,
  SaveCustomerAddressRequest,
  SaveCustomerContactRequest,
  SaveCustomerRequest
} from "../masterData/customerManagementService";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { EntityDetailPanel } from "../shared/masterData/EntityDetailPanel";
import { EntityFormPanel } from "../shared/masterData/EntityFormPanel";
import { EntityList } from "../shared/masterData/EntityList";
import { PageHeader } from "../shared/masterData/PageHeader";
import { SearchBar } from "../shared/masterData/SearchBar";
import { MasterDataListItem } from "../shared/masterData/types";

type CustomerFormState = {
  code: string;
  name: string;
  contacts: SaveCustomerContactRequest[];
  addresses: SaveCustomerAddressRequest[];
};

const emptyContact: SaveCustomerContactRequest = {
  name: "",
  email: "",
  phone: ""
};

const emptyAddress: SaveCustomerAddressRequest = {
  line1: "",
  line2: "",
  city: "",
  postalCode: "",
  country: "Angola"
};

const emptyForm: CustomerFormState = {
  code: "",
  name: "",
  contacts: [{ ...emptyContact }],
  addresses: [{ ...emptyAddress }]
};

export function CustomersPage() {
  const { session } = useAuth();
  const [customers, setCustomers] = useState<MasterDataListItem[]>([]);
  const [search, setSearch] = useState("");
  const [totalRecords, setTotalRecords] = useState(0);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);
  const [form, setForm] = useState<CustomerFormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  async function loadCustomers(nextSearch = search) {
    if (!session) {
      return;
    }

    const result = await customerManagementService.searchCustomers(session, nextSearch);
    setCustomers(result.items);
    setTotalRecords(result.totalRecords);
  }

  useEffect(() => {
    loadCustomers().catch(() => setError("Nao foi possivel carregar os clientes."));
  }, [session]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      await loadCustomers(search);
      setError(null);
    } catch {
      setError("Nao foi possivel pesquisar clientes.");
    }
  }

  async function handleSelectCustomer(item: MasterDataListItem) {
    if (!session) {
      return;
    }

    try {
      const customer = await customerManagementService.getCustomer(session, item.id);
      setSelectedCustomer(customer);
      setError(null);
    } catch {
      setError("Nao foi possivel carregar o detalhe do cliente.");
    }
  }

  async function handleSaveCustomer(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!session) {
      return;
    }

    const payload: SaveCustomerRequest = {
      code: form.code,
      name: form.name,
      contacts: form.contacts,
      addresses: form.addresses
    };

    try {
      const savedCustomer = editingCustomer
        ? await customerManagementService.updateCustomer(session, editingCustomer.id, payload)
        : await customerManagementService.createCustomer(session, payload);

      setSelectedCustomer(savedCustomer);
      setEditingCustomer(null);
      setForm(createEmptyForm());
      await loadCustomers();
      setError(null);
    } catch {
      setError("Nao foi possivel guardar o cliente.");
    }
  }

  async function handleDeactivateCustomer(customerId: string) {
    if (!session) {
      return;
    }

    try {
      await customerManagementService.deactivateCustomer(session, customerId);
      setSelectedCustomer(null);
      setEditingCustomer(null);
      await loadCustomers();
      setError(null);
    } catch {
      setError("Nao foi possivel desativar o cliente.");
    }
  }

  function beginEdit(customer: Customer) {
    setEditingCustomer(customer);
    setForm({
      code: customer.code,
      name: customer.name,
      contacts: customer.contacts.length
        ? customer.contacts.map((contact) => ({
            name: contact.name,
            email: contact.email ?? "",
            phone: contact.phone ?? ""
          }))
        : [{ ...emptyContact }],
      addresses: customer.addresses.length
        ? customer.addresses.map((address) => ({
            line1: address.line1,
            line2: address.line2 ?? "",
            city: address.city,
            postalCode: address.postalCode,
            country: address.country
          }))
        : [{ ...emptyAddress }]
    });
  }

  function updateContact(index: number, field: keyof SaveCustomerContactRequest, value: string) {
    setForm((current) => ({
      ...current,
      contacts: current.contacts.map((contact, contactIndex) =>
        contactIndex === index ? { ...contact, [field]: value } : contact
      )
    }));
  }

  function updateAddress(index: number, field: keyof SaveCustomerAddressRequest, value: string) {
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
        title="Clientes"
        description="Gestao de clientes, contactos e moradas para suportar vendas e operacoes futuras."
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
            items={customers}
            totalRecords={totalRecords}
            entityLabel="clientes"
            emptyMessage="Nenhum cliente encontrado."
            onSelect={handleSelectCustomer}
          />

          {selectedCustomer ? (
            <EntityDetailPanel
              code={selectedCustomer.code}
              name={selectedCustomer.name}
              isActive={selectedCustomer.isActive}
              onEdit={() => beginEdit(selectedCustomer)}
              onDeactivate={() => handleDeactivateCustomer(selectedCustomer.id)}
            >
              <div className="grid gap-4 text-sm md:grid-cols-2">
                <div>
                  <h4 className="font-semibold text-slate-700">Contactos</h4>
                  <div className="mt-2 space-y-2">
                    {selectedCustomer.contacts.map((contact) => (
                      <p key={contact.id} className="text-slate-600">
                        <span className="font-medium text-slate-900">{contact.name}</span>
                        {contact.email ? ` - ${contact.email}` : ""}
                        {contact.phone ? ` - ${contact.phone}` : ""}
                      </p>
                    ))}
                    {selectedCustomer.contacts.length === 0 ? (
                      <p className="text-slate-500">Sem contactos.</p>
                    ) : null}
                  </div>
                </div>
                <div>
                  <h4 className="font-semibold text-slate-700">Moradas</h4>
                  <div className="mt-2 space-y-2">
                    {selectedCustomer.addresses.map((address) => (
                      <p key={address.id} className="text-slate-600">
                        <span className="font-medium text-slate-900">{address.line1}</span>
                        {address.line2 ? `, ${address.line2}` : ""}, {address.city}, {address.postalCode},{" "}
                        {address.country}
                      </p>
                    ))}
                    {selectedCustomer.addresses.length === 0 ? (
                      <p className="text-slate-500">Sem moradas.</p>
                    ) : null}
                  </div>
                </div>
              </div>
            </EntityDetailPanel>
          ) : null}
        </div>

        <EntityFormPanel
          title={editingCustomer ? "Editar cliente" : "Criar cliente"}
          onSubmit={handleSaveCustomer}
          onCancel={
            editingCustomer
              ? () => {
                  setEditingCustomer(null);
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
              disabled={Boolean(editingCustomer)}
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

function createEmptyForm(): CustomerFormState {
  return {
    code: "",
    name: "",
    contacts: [{ ...emptyContact }],
    addresses: [{ ...emptyAddress }]
  };
}
