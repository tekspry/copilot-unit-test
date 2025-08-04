import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';

export const createAuthenticatedApiClient = (baseURL: string, getAccessToken: () => Promise<string>): AxiosInstance => {
  const client = axios.create({ baseURL });

  client.interceptors.request.use(async (config: AxiosRequestConfig) => {
    try {
      const token = await getAccessToken();
      if (token) {
        // Ensure headers object exists
        config.headers = config.headers || {};
        // TypeScript knows headers is defined now
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      console.error('Error getting access token:', error);
    }
    return config;
  });

  // Add response interceptor for handling 401/403 errors
  client.interceptors.response.use(
    (response) => response,
    async (error) => {
      if (error.response?.status === 401 || error.response?.status === 403) {
        // Handle token refresh or redirect to login
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );

  return client;
};
