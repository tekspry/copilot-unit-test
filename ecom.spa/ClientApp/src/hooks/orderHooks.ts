import { AxiosError, AxiosResponse } from "axios";
import { useNavigate } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "react-query";
import Config from "../config";
import { Order } from "../types/order";
import Problem from "../types/problem";
import { OrderDetails } from "../types/orderDetails";
import { createAuthenticatedApiClient } from "../utils/apiClient";
import { useAuth } from "./useAuth";

const getApiClient = () => {
  const { getAccessToken } = useAuth();
  const nav = useNavigate();

  const handleAuthError = (error: Error) => {
    console.error('Authentication error:', error);
    nav('/login');
    throw error;
  };

  return createAuthenticatedApiClient(Config.baseOrderApiUrl, async () => {
    try {
      return await getAccessToken();
    } catch (error) {
      handleAuthError(error as Error);
      // This throw is just for TypeScript - handleAuthError already throws
      throw error;
    }
  });
};

const useAddOrder = () => {
    const queryClient = useQueryClient();
    const nav = useNavigate();
    const apiClient = getApiClient();

    return useMutation<AxiosResponse, AxiosError<Problem>, Order>(
        (o) => apiClient.post(`/order`, o),
        {
          onSuccess: (resp, order) => {
            queryClient.invalidateQueries(["order", order.orderId]);
            nav("/");
          },
        }
      );
}

const useFetchOrderDetails = () => {
  const apiClient = getApiClient();
  
  return useQuery<OrderDetails[], AxiosError>("orders",  () =>
    apiClient.get(`/order`).then((resp) => resp.data)
  );
};

export { useAddOrder, useFetchOrderDetails };