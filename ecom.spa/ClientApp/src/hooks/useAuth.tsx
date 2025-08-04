import { createContext, useContext, useEffect, useState } from 'react';

interface AuthContextType {
  isAuthenticated: boolean;
  user: any | null;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  getAccessToken: () => Promise<string>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<any | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);

  // Initialize your authentication library here
  useEffect(() => {
    // Initialize auth library and check auth state
    // This is where you'd integrate with Auth0, Azure AD, etc.
  }, []);

  const login = async () => {
    // Implement login logic
    // This would typically trigger your auth provider's login flow
  };

  const logout = async () => {
    // Implement logout logic
    setIsAuthenticated(false);
    setUser(null);
    setAccessToken(null);
  };

  const getAccessToken = async () => {
    if (!accessToken) {
      // Implement token refresh logic here
      // This would typically call your auth provider's token endpoint
      throw new Error('No access token available');
    }
    return accessToken;
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout, getAccessToken }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
