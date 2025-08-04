using ecom.product.application.ProductApp;
using ecom.product.domain.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ecom.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _productApplication;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductApplication productApplication, ILogger<ProductController> logger)
        {
            _productApplication = productApplication;
            _logger = logger;
        }

        [HttpGet(Name = "GetProducts")]
        [AllowAnonymous] // Allow public access to product list
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            _logger.LogInformation("Getting all products");
            return Ok(await _productApplication.ListAsync());
        }

        [HttpGet("{id}", Name = "GetById")]
        [AllowAnonymous] // Allow public access to product details
        public async Task<ActionResult<Product>> GetById(string id)
        {
            _logger.LogInformation($"Getting product by id: {id}");
            
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Product ID is required");
            }

            try
            {
                var product = await _productApplication.GetAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return product;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost (Name = "product")]
        [Authorize(Roles = "Admin")] // Only admins can add products
        public async Task<ActionResult<string>> Add(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("Product name is required");
            }

            if (product.Name.Length > 255)
            {
                return BadRequest("Product name exceeds maximum length");
            }

            if (product.Price <= 0)
            {
                return BadRequest("Price must be greater than 0");
            }

            _logger.LogInformation($"Adding new product: {product.Name}");
            try
            {
                var id = await _productApplication.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("product/{id}/updatequantity/{quantity}")]
        [Authorize(Roles = "Admin")] // Only admins can update product quantity
        public async Task<ActionResult<int>> UpdateProductQuantity(string id, int quantity)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Product ID is required");
            }

            if (quantity < 0)
            {
                return BadRequest("Quantity cannot be negative");
            }

            _logger.LogInformation($"Updating product quantity: {id}, quantity: {quantity}");
            try
            {
                var result = await _productApplication.UpdateQuantityAsync(id, quantity);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Product not found: {ex.Message}");
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product quantity: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generateproductdescription")]
        [Authorize(Roles = "Admin")] // Only admins can generate product descriptions
        public async Task<ActionResult<Product>> GenerateProductDescription([FromBody] Product productDetails)
        {
            if (productDetails == null)
            {
                return BadRequest("Product cannot be null");
            }

            if (string.IsNullOrEmpty(productDetails.Name))
            {
                return BadRequest("Product name is required");
            }

            _logger.LogInformation($"Generating product description for: {productDetails.Name}");
            try
            {
                var result = await _productApplication.GenerateProductDescription(productDetails);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Product not found: {ex.Message}");
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating product description: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/rating")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductRatingResponse>> AddRating(string id, [FromBody] AddRatingRequest ratingRequest)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Product ID is required");
            }

            if (ratingRequest == null)
            {
                return BadRequest("Rating request cannot be null");
            }

            if (ratingRequest.Rating < 1 || ratingRequest.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5");
            }

            _logger.LogInformation($"Adding rating for product: {id}");
            try
            {
                var result = await _productApplication.AddRatingAsync(id, ratingRequest);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Product not found: {ex.Message}");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding rating: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/ratings")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductRatingResponse>>> GetRatings(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Product ID is required");
            }

            _logger.LogInformation($"Getting ratings for product: {id}");
            try
            {
                var ratings = await _productApplication.GetRatingsAsync(id);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting ratings: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}