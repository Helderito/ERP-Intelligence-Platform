import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem, PagedResult } from "../shared/masterData/types";

export type SupplierContact = {
  id: string;
  name: string;
  email: string | null;
  phone: string | null;
};

export type SupplierAddress = {
  id: string;
  line1: string;
  line2: string | null;
  city: string;
  postalCode: string;
  country: string;
};

export type Supplier = MasterDataListItem & {
  createdAtUtc: string;
  updatedAtUtc: string | null;
  deactivatedAtUtc: string | null;
  contacts: SupplierContact[];
  addresses: SupplierAddress[];
};

export type SaveSupplierContactRequest = {
  name: string;
  email: string;
  phone: string;
};

export type SaveSupplierAddressRequest = {
  line1: string;
  line2: string;
  city: string;
  postalCode: string;
  country: string;
};

export type SaveSupplierRequest = {
  code?: string;
  name: string;
  contacts: SaveSupplierContactRequest[];
  addresses: SaveSupplierAddressRequest[];
};

export const supplierManagementService = {
  searchSuppliers(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    });

    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return apiRequest<PagedResult<MasterDataListItem>>(
      session,
      `/suppliers?${parameters.toString()}`,
      {},
      "Nao foi possivel concluir o pedido de fornecedores."
    );
  },

  getSupplier(session: AuthenticationSession, supplierId: string) {
    return apiRequest<Supplier>(
      session,
      `/suppliers/${supplierId}`,
      {},
      "Nao foi possivel concluir o pedido de fornecedores."
    );
  },

  createSupplier(session: AuthenticationSession, supplier: SaveSupplierRequest) {
    return apiRequest<Supplier>(
      session,
      "/suppliers",
      {
        method: "POST",
        body: JSON.stringify(supplier)
      },
      "Nao foi possivel concluir o pedido de fornecedores."
    );
  },

  updateSupplier(session: AuthenticationSession, supplierId: string, supplier: SaveSupplierRequest) {
    return apiRequest<Supplier>(
      session,
      `/suppliers/${supplierId}`,
      {
        method: "PUT",
        body: JSON.stringify({
          name: supplier.name,
          contacts: supplier.contacts,
          addresses: supplier.addresses
        })
      },
      "Nao foi possivel concluir o pedido de fornecedores."
    );
  },

  deactivateSupplier(session: AuthenticationSession, supplierId: string) {
    return apiRequest<void>(
      session,
      `/suppliers/${supplierId}`,
      {
        method: "DELETE"
      },
      "Nao foi possivel concluir o pedido de fornecedores."
    );
  }
};
