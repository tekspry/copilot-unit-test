using Dapr.Client;
using ecom.order.domain.Order;
using ecom.order.infrastructure.Product;
using ecom.order.infrastructure.Services.Customer;
using Microsoft.Extensions.Logging;

namespace ecom.order.database.order
{
    public class OrderRepository : IOrderRepository
    {
        private List<Order> orders = new List<Order>();
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly DaprClient daprClient;
        private readonly ILogger<OrderRepository> logger;
        private const string cacheStoreName = "ordercache";

        public OrderRepository(ICustomerService customerService, IProductService productService,DaprClient daprClient, ILogger<OrderRepository> logger)
        {
            this._customerService = customerService;
            this._productService = productService;
            this.daprClient = daprClient;
            this.logger = logger;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            var key = $"orderlist";
            var orderList = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);

            return orderList;
        }

        public async Task<IEnumerable<OrderDetails>> GetOrderDetails()
        {
            var key = $"orderlist";
            var orderList = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);
            var orders = new List<OrderDetails>();

            foreach (var order in orderList)
            {
                var customerDetails = await _customerService.GetCustomerAsync(order.CustomerId);
                var productDetails = await _productService.GetProductAsync(order.ProductId);
                orders.Add(new OrderDetails()
                {
                    order = order,
                    CustomerDetails = customerDetails,
                    ProductDetails = productDetails
                });
            }

            return orders;
        }
        public async Task<Order> CreateOrder(Order order)
        {
            order.OrderId = Guid.NewGuid().ToString();
            var key = $"orderlist";
            var orders = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);

            if (orders == null)
                orders = new List<Order>();
                       
            orders.Add(order);
            
            await this.SaveOrderListToCacheStore(orders);

            return await Task.FromResult(order);
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var key = $"orderlist";
            var orders = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);
            var @order = orders.FirstOrDefault(e => e.OrderId == orderId);
            if (@order == null)
            {
                throw new InvalidOperationException("order not found");
            }
            return @order;
        }
        public async Task UpdateAsync(Order order)
        {
            var key = $"orderlist";
            var orders = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);

            var updatedOrder = orders.Where(x => x.OrderId == order.OrderId).FirstOrDefault();

            if (updatedOrder != null)
            {
                orders.Remove(updatedOrder);

                orders.Add(order);
            }

            await this.SaveOrderListToCacheStore(orders);
        }
        public async Task DeleteAsync(string orderId)
        {
            var key = $"orderlist";
            var orders = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);

            var updatedOrder = orders.Where(x => x.OrderId == orderId).FirstOrDefault();

            if (updatedOrder != null)
            {
                orders.Remove(updatedOrder);
            }

            await this.SaveOrderListToCacheStore(orders);
        }
        public async Task DeleteAllAsync()
        {   
            var key = $"orderlist";
            var orders = await daprClient.GetStateAsync<List<Order>>(cacheStoreName, key);
            orders.RemoveAll(orders.Contains);
            await this.SaveOrderListToCacheStore(orders);
        }


        private async Task SaveOrderListToCacheStore(List<Order> orders)
        {
            var key = $"orderlist";
            await daprClient.SaveStateAsync(cacheStoreName, key, orders);
            logger.LogInformation($"Created new order in cache store {cacheStoreName}");
        }
    }
}
