export type AuthenticationSession = {
  userId: string;
  email: string;
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  refreshToken: string;
  refreshTokenExpiresAtUtc: string;
};

type LoginRequest = {
  email: string;
  password: string;
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

async function postJson<TResponse>(path: string, body: unknown): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(body)
  });

  if (!response.ok) {
    throw new Error("Nao foi possivel concluir o pedido de autenticacao.");
  }

  return response.json() as Promise<TResponse>;
}

export const authenticationService = {
  login(request: LoginRequest) {
    return postJson<AuthenticationSession>("/auth/login", request);
  },

  refresh(refreshToken: string) {
    return postJson<AuthenticationSession>("/auth/refresh", { refreshToken });
  },

  async logout(refreshToken: string) {
    await fetch(`${apiBaseUrl}/auth/logout`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ refreshToken })
    });
  }
};
