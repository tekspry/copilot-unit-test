using ecom.notification.domain.Order;

namespace ecom.notification.application.Notification
{
    public interface INotificationApplication
    {
        Task SendNotificationAsync(Order order);
    }
}
