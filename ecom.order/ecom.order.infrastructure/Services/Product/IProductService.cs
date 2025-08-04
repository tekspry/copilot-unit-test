namespace ecom.order.infrastructure.Product
{
    public interface IProductService
    {
        Task<int> UpdateProductQuantity(string id, int quantity);
        Task<domain.Product.Product> GetProductAsync(string productId);
    }
}
