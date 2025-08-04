using ecom.order.application.Order;
using Microsoft.AspNetCore.Mvc;

namespace ecom.order.service.Controllers
{
    [ApiController]
    [Route("orderPaymentPendingJob")]    
    public class OrderPaymentPendingJobController : ControllerBase
    {
        private readonly ILogger<OrderPaymentPendingJobController> _logger;
        private readonly IOrderApplication _orderApplication;

        public OrderPaymentPendingJobController(ILogger<OrderPaymentPendingJobController> logger,
        IOrderApplication orderApplication)
        {
            this._logger = logger;
            this._orderApplication = orderApplication;
        }

        [HttpPost("", Name = "OrderPaymentPendingJob")]
        public async Task OnExecute()
        {   
            var orderProcessed = await _orderApplication.UpdateOrderPaymentPending();
        }
    }
}
