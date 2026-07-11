import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  Customer,
  customerManagementService,
  SaveCustomerAddressRequest,
  SaveCustomerContactRequest,
  SaveCustomerRequest
} from "../masterData/customerManagementService";

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
  const [customers, setCustomers] = useState<Customer[]>([]);
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
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Clientes</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Gestao de clientes, contactos e moradas para suportar vendas e operacoes futuras.
        </p>
      </div>

      {error ? <p className="text-sm font-medium text-red-700">{error}</p> : null}

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.2fr)_minmax(360px,0.9fr)]">
        <div className="space-y-4">
          <form className="flex gap-3" onSubmit={handleSearch}>
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Pesquisar por codigo ou nome"
              className="min-w-0 flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm"
            />
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
              Pesquisar
            </button>
          </form>

          <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
            <div className="border-b border-slate-200 px-4 py-3 text-sm text-slate-600">
              {totalRecords} clientes encontrados
            </div>
            <div className="divide-y divide-slate-200">
              {customers.map((customer) => (
                <button
                  key={customer.id}
                  type="button"
                  onClick={() => setSelectedCustomer(customer)}
                  className="grid w-full grid-cols-[120px_minmax(0,1fr)_100px] gap-4 px-4 py-3 text-left text-sm transition hover:bg-slate-50"
                >
                  <span className="font-mono font-semibold text-slate-900">{customer.code}</span>
                  <span className="truncate font-medium text-slate-800">{customer.name}</span>
                  <span className={customer.isActive ? "text-emerald-700" : "text-slate-500"}>
                    {customer.isActive ? "Ativo" : "Inativo"}
                  </span>
                </button>
              ))}
              {customers.length === 0 ? (
                <p className="px-4 py-6 text-sm text-slate-500">Nenhum cliente encontrado.</p>
              ) : null}
            </div>
          </div>

          {selectedCustomer ? (
            <article className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="font-mono text-sm font-semibold text-brand-700">{selectedCustomer.code}</p>
                  <h3 className="mt-1 text-xl font-semibold">{selectedCustomer.name}</h3>
                </div>
                <span className="rounded-md bg-slate-100 px-3 py-1 text-sm font-medium text-slate-700">
                  {selectedCustomer.isActive ? "Ativo" : "Inativo"}
                </span>
              </div>
              <div className="mt-4 grid gap-4 text-sm md:grid-cols-2">
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
                    {selectedCustomer.contacts.length === 0 ? <p className="text-slate-500">Sem contactos.</p> : null}
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
                    {selectedCustomer.addresses.length === 0 ? <p className="text-slate-500">Sem moradas.</p> : null}
                  </div>
                </div>
              </div>
              <div className="mt-4 flex gap-3">
                <button
                  type="button"
                  onClick={() => beginEdit(selectedCustomer)}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
                >
                  Editar
                </button>
                <button
                  type="button"
                  onClick={() => handleDeactivateCustomer(selectedCustomer.id)}
                  className="rounded-md border border-red-200 px-3 py-2 text-sm font-medium text-red-700"
                >
                  Desativar
                </button>
              </div>
            </article>
          ) : null}
        </div>

        <form className="space-y-5 rounded-md border border-slate-200 bg-white p-4 shadow-sm" onSubmit={handleSaveCustomer}>
          <h3 className="text-lg font-semibold">{editingCustomer ? "Editar cliente" : "Criar cliente"}</h3>
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
              onClick={() => setForm((current) => ({ ...current, contacts: [...current.contacts, { ...emptyContact }] }))}
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
              onClick={() => setForm((current) => ({ ...current, addresses: [...current.addresses, { ...emptyAddress }] }))}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
            >
              Adicionar morada
            </button>
          </fieldset>

          <div className="flex gap-3">
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
              Guardar
            </button>
            {editingCustomer ? (
              <button
                type="button"
                onClick={() => {
                  setEditingCustomer(null);
                  setForm(createEmptyForm());
                }}
                className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
              >
                Cancelar
              </button>
            ) : null}
          </div>
        </form>
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
