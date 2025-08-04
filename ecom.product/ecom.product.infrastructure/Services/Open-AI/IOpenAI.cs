using ecom.product.domain.Product;

namespace ecom.product.infrastructure.Services.Open_AI
{
    public interface IProductOpenAI
    {
        Task<Product> GenerateProductDescription(Product product);
    }
}
