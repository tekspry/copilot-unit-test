using ecom.order.domain.Order;

namespace ecom.order.database.order
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<OrderDetails>> GetOrderDetails();
        Task<Order> CreateOrder(Order order);
        Task<Order> GetOrderById(string orderId);
        Task UpdateAsync(Order order);
        Task DeleteAsync(string orderId);
        Task DeleteAllAsync();
    }
}
