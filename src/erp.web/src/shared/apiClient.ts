import { AuthenticationSession } from "../auth/authenticationService";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

export async function apiRequest<TResponse>(
  session: AuthenticationSession,
  path: string,
  options: RequestInit = {},
  errorMessage = "Nao foi possivel concluir o pedido."
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
    throw new Error(errorMessage);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}
