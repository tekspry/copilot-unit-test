using ecom.product.domain.Product;

namespace ecom.product.database.ProductDB
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductById(string productId);
        Task<string> CreateProduct(Product product);
        Task<int> UpdateProduct(Product product);
        Task<ProductRatingResponse> AddRatingAsync(string productId, AddRatingRequest ratingRequest);
        Task<IEnumerable<ProductRatingResponse>> GetRatingsAsync(string productId);
    }
}
