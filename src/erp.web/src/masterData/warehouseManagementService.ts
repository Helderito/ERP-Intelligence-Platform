import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem, PagedResult } from "../shared/masterData/types";

export type Warehouse = MasterDataListItem & {
  warehouseTypeId: string;
  warehouseTypeName: string;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  deactivatedAtUtc: string | null;
};

export type WarehouseType = {
  id: string;
  code: string;
  name: string;
};

export type SaveWarehouseRequest = {
  code?: string;
  name: string;
  warehouseTypeId: string;
};

const requestError = "Não foi possível concluir o pedido de armazéns.";

export const warehouseManagementService = {
  searchWarehouses(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return apiRequest<PagedResult<MasterDataListItem>>(
      session,
      `/warehouses?${parameters.toString()}`,
      {},
      requestError
    );
  },

  getWarehouse(session: AuthenticationSession, warehouseId: string) {
    return apiRequest<Warehouse>(session, `/warehouses/${warehouseId}`, {}, requestError);
  },

  createWarehouse(session: AuthenticationSession, warehouse: SaveWarehouseRequest) {
    return apiRequest<Warehouse>(
      session,
      "/warehouses",
      { method: "POST", body: JSON.stringify(warehouse) },
      requestError
    );
  },

  updateWarehouse(session: AuthenticationSession, warehouseId: string, warehouse: SaveWarehouseRequest) {
    return apiRequest<Warehouse>(
      session,
      `/warehouses/${warehouseId}`,
      {
        method: "PUT",
        body: JSON.stringify({ name: warehouse.name, warehouseTypeId: warehouse.warehouseTypeId })
      },
      requestError
    );
  },

  deactivateWarehouse(session: AuthenticationSession, warehouseId: string) {
    return apiRequest<void>(
      session,
      `/warehouses/${warehouseId}`,
      { method: "DELETE" },
      requestError
    );
  },

  getWarehouseTypes(session: AuthenticationSession) {
    return apiRequest<WarehouseType[]>(session, "/warehouse-types", {}, requestError);
  }
};
