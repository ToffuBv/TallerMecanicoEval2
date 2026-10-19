import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";
import { useQueryClient } from "@tanstack/react-query";
import { api, setToken } from "./api";
type Session = { name: string; role: string; expires: number };
const AuthContext = createContext<{
  session: Session | null;
  login: (name: string, password: string) => Promise<void>;
  logout: () => void;
}>(null!);
export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | null>(null);
  const cache = useQueryClient();
  function logout() {
    setToken(null);
    setSession(null);
    cache.clear();
  }
  async function login(name: string, password: string) {
    const { token } = await api.login(name, password);
    const payload = JSON.parse(
      new TextDecoder().decode(
        Uint8Array.from(
          atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")),
          (character) => character.charCodeAt(0),
        ),
      ),
    );
    const role =
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      payload.role;
    const expires = Number(payload.exp) * 1000;
    if (
      !Number.isFinite(expires) ||
      expires <= Date.now() ||
      typeof role !== "string"
    )
      throw new Error("La API devolvió una sesión inválida.");
    setToken(token);
    setSession({
      name:
        typeof payload[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ] === "string"
          ? payload[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            ]
          : name,
      role,
      expires,
    });
  }
  useEffect(() => {
    window.addEventListener("session-expired", logout);
    return () => window.removeEventListener("session-expired", logout);
  });
  useEffect(() => {
    if (session) {
      const timer = setTimeout(
        logout,
        Math.min(session.expires - Date.now(), 2147483647),
      );
      return () => clearTimeout(timer);
    }
  }, [session]);
  return (
    <AuthContext.Provider value={{ session, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
export const useAuth = () => useContext(AuthContext);
