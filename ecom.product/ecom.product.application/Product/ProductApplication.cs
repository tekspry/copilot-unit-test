using ecom.product.database.ProductDB;
using ecom.product.domain.Product;
using Microsoft.Extensions.Logging;
using ecom.product.infrastructure.Services.Open_AI;

namespace ecom.product.application.ProductApp
{
    public class ProductApplication : IProductApplication
    {
        private readonly IProductRepository _productRepository;
      
        private readonly ILogger<ProductApplication> logger;
        private readonly IProductOpenAI _openAI;

        public ProductApplication(IProductRepository productRepository, ILogger<ProductApplication> logger, IProductOpenAI openAI)
        {
            _productRepository = productRepository;            
            this.logger = logger;
            this._openAI = openAI;
        }

        public async Task<domain.Product.Product> GetAsync(string id)
        {
            return await _productRepository.GetProductById(id);
        }

        public async Task<IEnumerable<domain.Product.Product>> ListAsync()
        {           
            return await _productRepository.GetProducts();
        }

        public async Task<string> AddAsync(Product product)
        {   
            return await _productRepository.CreateProduct(product);
        }

        public async Task<int> UpdateQuantityAsync(string id, int quantity) 
        {
            var product = await GetAsync(id);
            product.Quantity -= quantity;
            return await _productRepository.UpdateProduct(product);
        }

        public async Task<Product> GenerateProductDescription(Product product)
        {
            return await _openAI.GenerateProductDescription(product);
        }

        public async Task<ProductRatingResponse> AddRatingAsync(string productId, AddRatingRequest ratingRequest)
        {
            return await _productRepository.AddRatingAsync(productId, ratingRequest);
        }

        public async Task<IEnumerable<ProductRatingResponse>> GetRatingsAsync(string productId)
        {
            return await _productRepository.GetRatingsAsync(productId);
        }
    }
}