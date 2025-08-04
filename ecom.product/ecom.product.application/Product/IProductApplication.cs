using ecom.product.domain.Product;

namespace ecom.product.application.ProductApp
{
    public interface IProductApplication
    {
        Task<Product> GetAsync(string id);
        Task<IEnumerable<Product>> ListAsync();
        Task<string> AddAsync(Product product);
        Task<int> UpdateQuantityAsync(string id, int quantity);
        Task<Product> GenerateProductDescription(Product product);
        Task<ProductRatingResponse> AddRatingAsync(string productId, AddRatingRequest ratingRequest);
        Task<IEnumerable<ProductRatingResponse>> GetRatingsAsync(string productId);
    }
}
