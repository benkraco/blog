import { createContext, useContext, useState } from "react";
import * as authService from "../services/authApi";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  const isAuthenticated = user !== null;

  const login = async (username, password) => {
    await authService.login(username, password);

    setUser({ username });
  };

  const logout = async () => {
    await authService.logout();

    setUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuthContext() {
  return useContext(AuthContext);
}
