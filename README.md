# Github Copilot Documentation Codebase

This application is intended to demonstrate the basics of using Github copilot for documentation. It is a demo project for blog series on AI Code Genertors tools, by Gagan Bajaj.

This version of the code is using Dapr 1.9

## Running the app development environment

**Prerequisites:** We need to have the [Dapr CLI installed](https://docs.dapr.io/getting-started/install-dapr-cli/), as well as Docker installed (e.g. Docker Desktop for Windows), and to have set up dapr in self-hosted mode with `dapr init`

Open 6 terminal windows. 
In the `ecom.product.service` folder run `start-product-app.ps1`, inside `ecom.order.service` folder run `start-order-app.ps1`, inside `ecom.payment.service` folder run `start-payment-app.ps1`, inside `ecom.notification.service` folder run `start-notification-app.ps1`, inside `ecom.customer.service` folder run `start-customer-app.ps1` and also run `npm start` in ecom.spa react app. 

### Port Details ###
The ports used are specified in the PowerShell start up scripts. 

Product service : `http://localhost:5016/` 
Order service : `http://localhost:5206/` 
Payment service : `http://localhost:5293/` 
Notification service : `http://localhost:5294/`
Customer service : `http://localhost:5295/`
Frontend app : `http://localhost:3000`
