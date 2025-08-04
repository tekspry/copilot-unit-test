using ecom.notification.infrastructure.Extensions;

namespace ecom.notification.infrastructure.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly HttpClient client;
        public ProductService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<domain.Product.Product> GetProductAsync(string productId)
        {
            var response = await client.GetAsync($"product/{productId}");
            return await response.ReadContentAs<domain.Product.Product>();

        }
    }
}
