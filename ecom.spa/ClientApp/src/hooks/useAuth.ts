import { useState, useCallback } from 'react';

interface AuthState {
  token: string | null;
}

export const useAuth = () => {
  const [auth, setAuth] = useState<AuthState>({
    token: localStorage.getItem('auth_token')
  });

  const getAccessToken = useCallback(async (): Promise<string> => {
    // For development, return the token from localStorage
    // In production, you might want to add token refresh logic here
    if (!auth.token) {
      throw new Error('No access token available');
    }
    return auth.token;
  }, [auth.token]);

  const login = useCallback(async (token: string) => {
    localStorage.setItem('auth_token', token);
    setAuth({ token });
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('auth_token');
    setAuth({ token: null });
  }, []);

  return {
    isAuthenticated: !!auth.token,
    getAccessToken,
    login,
    logout
  };
};
