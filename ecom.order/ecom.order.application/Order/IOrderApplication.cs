using ecom.order.domain.Order;

namespace ecom.order.application.Order
{
    public interface IOrderApplication
    {
        Task<IEnumerable<domain.Order.Order>> ListAsync();
        Task<IEnumerable<domain.Order.OrderDetails>> ListOrderDetailsAsync();
        Task<domain.Order.Order> AddAsync(domain.Order.Order order);
        Task<domain.Order.Order> GetAsync(string id);
        Task UpdateAsync(domain.Order.Order order);
        Task<int> UpdateOrderPaymentPending();
        Task DeleteAsync(string id);
        Task DeleteAllAsync();
    }
}
