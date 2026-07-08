import { createContext, useCallback, useEffect, useMemo, useState } from "react";
import { AuthenticationSession, authenticationService } from "./authenticationService";

type AuthContextValue = {
  session: AuthenticationSession | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
};

export const AuthContext = createContext<AuthContextValue | null>(null);

const storageKey = "erp.authentication.session";
const refreshSkewMs = 60_000;

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [session, setSession] = useState<AuthenticationSession | null>(() => {
    const storedValue = window.localStorage.getItem(storageKey);

    return storedValue ? (JSON.parse(storedValue) as AuthenticationSession) : null;
  });

  const persistSession = useCallback((nextSession: AuthenticationSession | null) => {
    setSession(nextSession);

    if (nextSession) {
      window.localStorage.setItem(storageKey, JSON.stringify(nextSession));
      return;
    }

    window.localStorage.removeItem(storageKey);
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const nextSession = await authenticationService.login({ email, password });
      persistSession(nextSession);
    },
    [persistSession]
  );

  const logout = useCallback(async () => {
    const currentSession = session;
    persistSession(null);

    if (currentSession?.refreshToken) {
      await authenticationService.logout(currentSession.refreshToken);
    }
  }, [persistSession, session]);

  useEffect(() => {
    if (!session) {
      return;
    }

    const refreshInMs = Math.max(
      new Date(session.accessTokenExpiresAtUtc).getTime() - Date.now() - refreshSkewMs,
      1_000
    );

    const timeoutId = window.setTimeout(async () => {
      try {
        const nextSession = await authenticationService.refresh(session.refreshToken);
        persistSession(nextSession);
      } catch {
        persistSession(null);
      }
    }, refreshInMs);

    return () => window.clearTimeout(timeoutId);
  }, [persistSession, session]);

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: Boolean(session),
      login,
      logout
    }),
    [login, logout, session]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
