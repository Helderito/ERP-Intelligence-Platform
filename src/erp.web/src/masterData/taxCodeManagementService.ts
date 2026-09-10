import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem } from "../shared/masterData/types";

export type TaxCode = MasterDataListItem & {
  rate: number;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  deactivatedAtUtc: string | null;
};

export type SaveTaxCodeRequest = { code?: string; name: string; rate: number };

const requestError = "Não foi possível concluir o pedido de códigos de imposto.";

export const taxCodeManagementService = {
  list(session: AuthenticationSession, search: string) {
    const parameters = new URLSearchParams({ includeInactive: "true" });
    if (search.trim()) {
      parameters.set("search", search.trim());
    }
    return apiRequest<TaxCode[]>(session, `/tax-codes?${parameters}`, {}, requestError);
  },
  create(session: AuthenticationSession, taxCode: SaveTaxCodeRequest) {
    return apiRequest<TaxCode>(
      session,
      "/tax-codes",
      { method: "POST", body: JSON.stringify(taxCode) },
      requestError
    );
  },
  update(session: AuthenticationSession, id: string, taxCode: SaveTaxCodeRequest) {
    return apiRequest<TaxCode>(
      session,
      `/tax-codes/${id}`,
      { method: "PUT", body: JSON.stringify({ name: taxCode.name, rate: taxCode.rate }) },
      requestError
    );
  },
  deactivate(session: AuthenticationSession, id: string) {
    return apiRequest<void>(session, `/tax-codes/${id}`, { method: "DELETE" }, requestError);
  }
};
