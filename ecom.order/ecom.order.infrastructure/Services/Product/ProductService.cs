using ecom.order.Extensions;

namespace ecom.order.infrastructure.Product
{
    public class ProductService : IProductService
    {
        private readonly HttpClient client;
        public ProductService(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));            
        }

        public async Task<int> UpdateProductQuantity(string id, int quantity)
        {  
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(id));
            }

            try 
            {
                var response = await client.PostAsJson<string>($"product/product/{id}/updatequantity/{quantity}", string.Empty);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to update product quantity. Status: {response.StatusCode}");
                }

                var productPrice = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(productPrice))
                {
                    throw new InvalidOperationException("Product price was not returned from the service");
                }

                return Convert.ToInt32(productPrice);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Failed to update product quantity: {ex.Message}", ex);
            }
        }

        public async Task<domain.Product.Product> GetProductAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
            }

            try
            {
                var response = await client.GetAsync($"product/{productId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to get product. Status: {response.StatusCode}");
                }

                return await response.ReadContentAs<domain.Product.Product>()
                    ?? throw new InvalidOperationException($"Product not found with ID: {productId}");
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Failed to get product: {ex.Message}", ex);
            }
        }
    }
}
