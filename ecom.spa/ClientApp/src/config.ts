const requiredEnvVar = (name: string): string => {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }
  return value;
};

const Config = {
  baseProductApiUrl: requiredEnvVar('REACT_APP_PRODUCT_API_URL'),
  baseOrderApiUrl: requiredEnvVar('REACT_APP_ORDER_API_URL'),
  basePaymentApiUrl: requiredEnvVar('REACT_APP_PAYMENT_API_URL'),
  baseNotificationApiUrl: requiredEnvVar('REACT_APP_NOTIFICATION_API_URL'),
  baseCustomerApiUrl: requiredEnvVar('REACT_APP_CUSTOMER_API_URL'),
  auth: {
    domain: requiredEnvVar('REACT_APP_AUTH_DOMAIN'),
    clientId: requiredEnvVar('REACT_APP_AUTH_CLIENT_ID'),
    audience: requiredEnvVar('REACT_APP_AUTH_AUDIENCE'),
  }
};

const currencyFormatter = Intl.NumberFormat("en-US", {
  style: "currency",
  currency: "USD",
  maximumFractionDigits: 0,
});

export default Config;
export { currencyFormatter };
