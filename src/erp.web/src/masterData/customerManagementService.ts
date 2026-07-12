import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem, PagedResult } from "../shared/masterData/types";

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

export type Customer = MasterDataListItem & {
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

export const customerManagementService = {
  searchCustomers(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    });

    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return apiRequest<PagedResult<MasterDataListItem>>(
      session,
      `/customers?${parameters.toString()}`,
      {},
      "Nao foi possivel concluir o pedido de clientes."
    );
  },

  getCustomer(session: AuthenticationSession, customerId: string) {
    return apiRequest<Customer>(
      session,
      `/customers/${customerId}`,
      {},
      "Nao foi possivel concluir o pedido de clientes."
    );
  },

  createCustomer(session: AuthenticationSession, customer: SaveCustomerRequest) {
    return apiRequest<Customer>(
      session,
      "/customers",
      {
        method: "POST",
        body: JSON.stringify(customer)
      },
      "Nao foi possivel concluir o pedido de clientes."
    );
  },

  updateCustomer(session: AuthenticationSession, customerId: string, customer: SaveCustomerRequest) {
    return apiRequest<Customer>(
      session,
      `/customers/${customerId}`,
      {
        method: "PUT",
        body: JSON.stringify({
          name: customer.name,
          contacts: customer.contacts,
          addresses: customer.addresses
        })
      },
      "Nao foi possivel concluir o pedido de clientes."
    );
  },

  deactivateCustomer(session: AuthenticationSession, customerId: string) {
    return apiRequest<void>(
      session,
      `/customers/${customerId}`,
      {
        method: "DELETE"
      },
      "Nao foi possivel concluir o pedido de clientes."
    );
  }
};
