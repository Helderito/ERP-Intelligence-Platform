import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem } from "../shared/masterData/types";

export type SaveReferenceDataRequest = { code?: string; name: string };

export type ManagedReferenceDataService = {
  list: (session: AuthenticationSession, search: string) => Promise<MasterDataListItem[]>;
  create: (session: AuthenticationSession, item: SaveReferenceDataRequest) => Promise<MasterDataListItem>;
  update: (
    session: AuthenticationSession,
    id: string,
    item: SaveReferenceDataRequest
  ) => Promise<MasterDataListItem>;
  deactivate: (session: AuthenticationSession, id: string) => Promise<void>;
};

export function createManagedReferenceDataService(
  route: string,
  errorMessage: string
): ManagedReferenceDataService {
  return {
    list(session, search) {
      const parameters = new URLSearchParams({ includeInactive: "true" });
      if (search.trim()) {
        parameters.set("search", search.trim());
      }
      return apiRequest<MasterDataListItem[]>(session, `${route}?${parameters}`, {}, errorMessage);
    },
    create(session, item) {
      return apiRequest<MasterDataListItem>(
        session,
        route,
        { method: "POST", body: JSON.stringify(item) },
        errorMessage
      );
    },
    update(session, id, item) {
      return apiRequest<MasterDataListItem>(
        session,
        `${route}/${id}`,
        { method: "PUT", body: JSON.stringify({ name: item.name }) },
        errorMessage
      );
    },
    deactivate(session, id) {
      return apiRequest<void>(session, `${route}/${id}`, { method: "DELETE" }, errorMessage);
    }
  };
}
