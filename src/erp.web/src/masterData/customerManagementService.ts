import { AuthenticationSession } from "../auth/authenticationService";
import { PagedResult } from "./productCatalogService";

export type CustomerContact = {
  id: string;
  name: string;
  email: string | null;
  phone: string | null;
};

export type CustomerAddress = {
  id: string;
  line1: string;
  line2: string | null;
  city: string;
  postalCode: string;
  country: string;
};

export type Customer = {
  id: string;
  code: string;
  name: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  deactivatedAtUtc: string | null;
  contacts: CustomerContact[];
  addresses: CustomerAddress[];
};

export type SaveCustomerContactRequest = {
  name: string;
  email: string;
  phone: string;
};

export type SaveCustomerAddressRequest = {
  line1: string;
  line2: string;
  city: string;
  postalCode: string;
  country: string;
};

export type SaveCustomerRequest = {
  code?: string;
  name: string;
  contacts: SaveCustomerContactRequest[];
  addresses: SaveCustomerAddressRequest[];
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

async function request<TResponse>(
  session: AuthenticationSession,
  path: string,
  options: RequestInit = {}
): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
      ...options.headers
    }
  });

  if (!response.ok) {
    throw new Error("Nao foi possivel concluir o pedido de clientes.");
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}

export const customerManagementService = {
  searchCustomers(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    });

    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return request<PagedResult<Customer>>(session, `/customers?${parameters.toString()}`);
  },

  getCustomer(session: AuthenticationSession, customerId: string) {
    return request<Customer>(session, `/customers/${customerId}`);
  },

  createCustomer(session: AuthenticationSession, customer: SaveCustomerRequest) {
    return request<Customer>(session, "/customers", {
      method: "POST",
      body: JSON.stringify(customer)
    });
  },

  updateCustomer(session: AuthenticationSession, customerId: string, customer: SaveCustomerRequest) {
    return request<Customer>(session, `/customers/${customerId}`, {
      method: "PUT",
      body: JSON.stringify({
        name: customer.name,
        contacts: customer.contacts,
        addresses: customer.addresses
      })
    });
  },

  deactivateCustomer(session: AuthenticationSession, customerId: string) {
    return request<void>(session, `/customers/${customerId}`, {
      method: "DELETE"
    });
  }
};
