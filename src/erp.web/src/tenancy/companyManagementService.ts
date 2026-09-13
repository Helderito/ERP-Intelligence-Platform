import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";

export type Establishment = {
  id: string;
  code: string;
  name: string;
  establishmentNumber: string;
};

export type CompanyFiscalProfile = {
  nif: string;
  vatRegime: string;
  fiscalAddress: string;
};

export type Company = {
  id: string;
  name: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  fiscalProfile: CompanyFiscalProfile | null;
  establishments: Establishment[];
};

const requestError = "Não foi possível concluir o pedido da empresa.";

export const companyManagementService = {
  getCompany(session: AuthenticationSession, companyId: string) {
    return apiRequest<Company>(session, `/companies/${companyId}`, {}, requestError);
  },

  updateCompany(session: AuthenticationSession, companyId: string, name: string) {
    return apiRequest<Company>(
      session,
      `/companies/${companyId}`,
      { method: "PUT", body: JSON.stringify({ name }) },
      requestError
    );
  },

  updateFiscalProfile(
    session: AuthenticationSession,
    companyId: string,
    profile: CompanyFiscalProfile
  ) {
    return apiRequest<Company>(
      session,
      `/companies/${companyId}/fiscal-profile`,
      { method: "PUT", body: JSON.stringify(profile) },
      requestError
    );
  },

  addEstablishment(
    session: AuthenticationSession,
    companyId: string,
    establishment: Omit<Establishment, "id">
  ) {
    return apiRequest<Company>(
      session,
      `/companies/${companyId}/establishments`,
      { method: "POST", body: JSON.stringify(establishment) },
      requestError
    );
  }
};
