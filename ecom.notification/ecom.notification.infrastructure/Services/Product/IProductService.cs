namespace ecom.notification.infrastructure.Services.Product
{
    public interface IProductService
    {
        Task<notification.domain.Product.Product> GetProductAsync(string productId);
    }
}
