using ecom.notification.domain.Notification;

namespace ecom.notification.infrastructure.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(OrderForNotfication order);
    }
}
