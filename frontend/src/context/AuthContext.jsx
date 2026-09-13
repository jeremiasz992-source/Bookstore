import { createContext, useContext, useMemo, useState } from "react";
import { login as loginRequest, register as registerRequest } from "../api/auth";

const AuthContext = createContext(null);

const storageKey = "bookstore_auth";
const tokenKey = "bookstore_token";

function readStoredUser() {
  const value = localStorage.getItem(storageKey);
  return value ? JSON.parse(value) : null;
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(readStoredUser);

  async function signIn(payload) {
    const response = await loginRequest(payload);
    localStorage.setItem(tokenKey, response.token);
    localStorage.setItem(storageKey, JSON.stringify(response));
    setUser(response);
    return response;
  }

  async function signUp(payload) {
    const response = await registerRequest(payload);
    localStorage.setItem(tokenKey, response.token);
    localStorage.setItem(storageKey, JSON.stringify(response));
    setUser(response);
    return response;
  }

  function signOut() {
    localStorage.removeItem(tokenKey);
    localStorage.removeItem(storageKey);
    setUser(null);
  }

  const value = useMemo(
    () => ({
      user,
      isAuthenticated: Boolean(user?.token),
      isAdmin: user?.role === "Admin",
      signIn,
      signUp,
      signOut
    }),
    [user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  return useContext(AuthContext);
}
