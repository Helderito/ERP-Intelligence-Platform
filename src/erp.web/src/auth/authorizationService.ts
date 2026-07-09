import { AuthenticationSession } from "./authenticationService";

export type Role = {
  id: string;
  name: string;
  isActive: boolean;
  permissionIds: string[];
};

export type Permission = {
  id: string;
  code: string;
  description: string;
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
    throw new Error("Nao foi possivel concluir o pedido de autorizacao.");
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}

export const authorizationService = {
  getRoles(session: AuthenticationSession) {
    return request<Role[]>(session, "/roles");
  },

  createRole(session: AuthenticationSession, name: string) {
    return request<Role>(session, "/roles", {
      method: "POST",
      body: JSON.stringify({ name })
    });
  },

  updateRole(session: AuthenticationSession, roleId: string, name: string) {
    return request<Role>(session, `/roles/${roleId}`, {
      method: "PUT",
      body: JSON.stringify({ name })
    });
  },

  deactivateRole(session: AuthenticationSession, roleId: string) {
    return request<void>(session, `/roles/${roleId}`, {
      method: "DELETE"
    });
  },

  getPermissions(session: AuthenticationSession) {
    return request<Permission[]>(session, "/permissions");
  },

  assignPermissions(session: AuthenticationSession, roleId: string, permissionIds: string[]) {
    return request<Role>(session, `/roles/${roleId}/permissions`, {
      method: "POST",
      body: JSON.stringify({ permissionIds })
    });
  },

  assignRolesToUser(session: AuthenticationSession, userId: string, roleIds: string[]) {
    return request<void>(session, `/users/${userId}/roles`, {
      method: "POST",
      body: JSON.stringify({ roleIds })
    });
  }
};
