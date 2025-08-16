#if false
using ecom.product.domain.Product;

namespace ecom.product.application.ProductApp
{
    // Duplicate interface file kept for reference. Disabled to avoid duplicate-type errors.
    public interface IProductApplication
    {
        // ...existing method signatures...

        // Rating APIs required by tests
        Task<ProductRatingResponse> AddRatingAsync(string productId, AddRatingRequest request);
        Task<IEnumerable<ProductRatingResponse>> GetRatingsAsync(string productId);
    }
}
#endif