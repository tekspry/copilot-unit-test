using ecom.product.application.ProductApp;
using ecom.product.domain.Product;
using ecom.ProductService.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using ecom.product.testing.Builders;

namespace ecom.product.testing.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductApplication> _mockProductApplication;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductApplication = new Mock<IProductApplication>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductApplication.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult()
        {
            // Arrange
            var expectedProducts = new List<Product> 
            { 
                new Product 
                { 
                    ProductId = Guid.NewGuid().ToString(),
                    Name = "Test Product",
                    Price = 100,
                    Seller = "Test Seller",
                    AvailableSince = "2023-12-20",
                    ShortDescription = "Short Test Description",
                    ImageUrl = "http://test.com/image.jpg",
                    Quantity = 10,
                    ProductDescription = "Full Test Description"
                } 
            };
            _mockProductApplication.Setup(x => x.ListAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var products = okResult.Value.ShouldBeAssignableTo<IEnumerable<Product>>();
            products.Count().ShouldBe(1);

            // Verify logger was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("Getting all products", o.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAll_WithMultipleProducts_ShouldReturnOkResult()
        {
            // Arrange
            var expectedProducts = new List<Product> 
            { 
                ProductBuilder.Create()
                    .WithName("Gaming Laptop")
                    .WithPrice(1299)
                    .WithQuantity(5)
                    .Build(),
                ProductBuilder.Create()
                    .WithName("Wireless Mouse")
                    .WithPrice(49)
                    .WithQuantity(100)
                    .Build()
            };
            _mockProductApplication.Setup(x => x.ListAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var products = okResult.Value.ShouldBeAssignableTo<IEnumerable<Product>>();
            products.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetAll_WithEmptyList_ShouldReturnOkResultWithEmptyList()
        {
            // Arrange
            var expectedProducts = new List<Product>();
            _mockProductApplication.Setup(x => x.ListAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var products = okResult.Value.ShouldBeAssignableTo<IEnumerable<Product>>();
            products.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var expectedProduct = new Product 
            { 
                ProductId = productId,
                Name = "Test Product",
                Price = 100,
                Seller = "Test Seller",
                AvailableSince = "2023-12-20",
                ShortDescription = "Short Test Description",
                ImageUrl = "http://test.com/image.jpg",
                Quantity = 10,
                ProductDescription = "Full Test Description"
            };

            _mockProductApplication.Setup(x => x.GetAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var product = result.Value.ShouldBeOfType<Product>();
            product.ProductId.ShouldBe(productId);
            
            // Verify logger was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Getting product by id: {productId}", o.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            _mockProductApplication.Setup(x => x.GetAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Add_WithValidProduct_ShouldReturnCreatedResponse()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var newProduct = new Product 
            { 
                Name = "New Product",
                Price = 150,
                Seller = "Test Seller",
                ShortDescription = "New Short Description",
                ImageUrl = "http://test.com/newimage.jpg",
                Quantity = 5,
                ProductDescription = "New Full Description"
            };

            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var createdAtActionResult = result.Result.ShouldBeOfType<CreatedAtActionResult>();
            createdAtActionResult.ActionName.ShouldBe(nameof(ProductController.GetById));
            createdAtActionResult.RouteValues["id"].ShouldBe(productId);
        }

        [Fact]
        public async Task Add_WithZeroQuantity_ShouldStillCreateProduct()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var newProduct = ProductBuilder.Create()
                .WithQuantity(0)
                .WithPrice(50)
                .Build();

            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var createdAtActionResult = result.Result.ShouldBeOfType<CreatedAtActionResult>();
            createdAtActionResult.ActionName.ShouldBe(nameof(ProductController.GetById));
            createdAtActionResult.RouteValues["id"].ShouldBe(productId);
        }

        [Fact]
        public async Task Add_WithMaxValues_ShouldCreateProduct()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var newProduct = ProductBuilder.Create()
                .WithName(new string('A', 255)) // Max length test
                .WithPrice(int.MaxValue)
                .WithQuantity(int.MaxValue)
                .Build();

            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var createdAtActionResult = result.Result.ShouldBeOfType<CreatedAtActionResult>();
            createdAtActionResult.ActionName.ShouldBe(nameof(ProductController.GetById));
            createdAtActionResult.RouteValues["id"].ShouldBe(productId);
        }

        [Fact]
        public async Task Add_WhenExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            var newProduct = ProductBuilder.Create()
                .WithName("Test Product")
                .WithPrice(100) // Setting a valid price to avoid price validation error
                .Build();
            var errorMessage = "Error adding product";
            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task UpdateProductQuantity_WithValidData_ShouldReturnUpdatedQuantity()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var newQuantity = 15;
            _mockProductApplication.Setup(x => x.UpdateQuantityAsync(productId, newQuantity))
                .ReturnsAsync(newQuantity);

            // Act
            var result = await _controller.UpdateProductQuantity(productId, newQuantity);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(newQuantity);
        }

        [Fact]
        public async Task UpdateProductQuantity_WithNegativeQuantity_ShouldReturnBadRequest()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var errorMessage = "Quantity cannot be negative";
            _mockProductApplication.Setup(x => x.UpdateQuantityAsync(It.IsAny<string>(), -1))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.UpdateProductQuantity(productId, -1);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task UpdateProductQuantity_WhenExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var errorMessage = "Error updating quantity";
            _mockProductApplication.Setup(x => x.UpdateQuantityAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.UpdateProductQuantity(productId, 10);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task GenerateProductDescription_WithValidProduct_ShouldReturnUpdatedProduct()
        {
            // Arrange
            var productDetails = new Product 
            { 
                ProductId = Guid.NewGuid().ToString(),
                Name = "Test Product",
                ShortDescription = "Short Description"
            };

            var updatedProduct = new Product 
            {
                ProductId = productDetails.ProductId,
                Name = productDetails.Name,
                ShortDescription = productDetails.ShortDescription,
                ProductDescription = "Generated Description"
            };

            _mockProductApplication.Setup(x => x.GenerateProductDescription(It.IsAny<Product>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.GenerateProductDescription(productDetails);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var returnedProduct = okResult.Value.ShouldBeOfType<Product>();
            returnedProduct.ProductDescription.ShouldBe("Generated Description");
        }

        [Fact]
        public async Task GenerateProductDescription_WithEmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var productDetails = ProductBuilder.Create()
                .WithName(string.Empty)
                .Build();

            var errorMessage = "Product name is required";
            _mockProductApplication.Setup(x => x.GenerateProductDescription(It.IsAny<Product>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.GenerateProductDescription(productDetails);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task GenerateProductDescription_WhenExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            var productDetails = new Product { Name = "Test Product" };
            var errorMessage = "Error generating description";
            _mockProductApplication.Setup(x => x.GenerateProductDescription(It.IsAny<Product>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.GenerateProductDescription(productDetails);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public void Controller_ShouldHaveAuthorizeAttribute()
        {
            // Arrange & Act
            var attributes = typeof(ProductController).GetCustomAttributes(true);

            // Assert
            attributes.ShouldContain(x => x is AuthorizeAttribute);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Add_WithInvalidName_ShouldReturnBadRequest(string name)
        {
            // Arrange
            var newProduct = ProductBuilder.Create()
                .WithName(name)
                .Build();

            var errorMessage = "Product name is required";
            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Add_WithInvalidPrice_ShouldReturnBadRequest(int price)
        {
            // Arrange
            var newProduct = ProductBuilder.Create()
                .WithPrice(price)
                .Build();

            var errorMessage = "Price must be greater than 0";
            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task UpdateProductQuantity_WithInvalidProductId_ShouldReturnBadRequest(string productId)
        {
            // Arrange
            var errorMessage = "Product ID is required";
            _mockProductApplication.Setup(x => x.UpdateQuantityAsync(productId, It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.UpdateProductQuantity(productId, 10);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task GenerateProductDescription_WithNullProduct_ShouldReturnBadRequest()
        {
            // Arrange
            Product nullProduct = null;

            // Act
            var result = await _controller.GenerateProductDescription(nullProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe("Product cannot be null");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetById_WithInvalidProductId_ShouldReturnBadRequest(string productId)
        {
            // Arrange
            var errorMessage = "Product ID is required";
            _mockProductApplication.Setup(x => x.GetAsync(productId))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task Add_WithNullProduct_ShouldReturnBadRequest()
        {
            // Arrange
            Product nullProduct = null;

            // Act
            var result = await _controller.Add(nullProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe("Product cannot be null");
        }

        [Fact]
        public async Task Add_WithExcessiveNameLength_ShouldReturnBadRequest()
        {
            // Arrange
            var newProduct = ProductBuilder.Create()
                .WithName(new string('A', 256)) // Exceeding max length
                .Build();

            var errorMessage = "Product name exceeds maximum length";
            _mockProductApplication.Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.Add(newProduct);

            // Assert
            var badRequestResult = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task UpdateProductQuantity_ProductNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var errorMessage = "Product not found";
            _mockProductApplication.Setup(x => x.UpdateQuantityAsync(productId, It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            // Act
            var result = await _controller.UpdateProductQuantity(productId, 10);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GenerateProductDescription_ProductNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var product = ProductBuilder.Create().Build();
            var errorMessage = "Product not found";
            _mockProductApplication.Setup(x => x.GenerateProductDescription(It.IsAny<Product>()))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            // Act
            var result = await _controller.GenerateProductDescription(product);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddRating_MethodExists()
        {
            // Arrange & Act
            var methodInfo = typeof(ProductController).GetMethod("AddRating");

            // Assert
            methodInfo.ShouldNotBeNull("AddRating method should exist");
            methodInfo.ReturnType.ShouldBe(typeof(Task<ActionResult<ProductRatingResponse>>));
            var parameters = methodInfo.GetParameters();
            parameters.Length.ShouldBe(2);
            parameters[0].ParameterType.ShouldBe(typeof(string));
            parameters[1].ParameterType.ShouldBe(typeof(AddRatingRequest));
        }

        [Fact]
        public void GetRatings_MethodExists()
        {
            // Arrange & Act
            var methodInfo = typeof(ProductController).GetMethod("GetRatings");

            // Assert
            methodInfo.ShouldNotBeNull("GetRatings method should exist");
            methodInfo.ReturnType.ShouldBe(typeof(Task<ActionResult<IEnumerable<ProductRatingResponse>>>));
            var parameters = methodInfo.GetParameters();
            parameters.Length.ShouldBe(1);
            parameters[0].ParameterType.ShouldBe(typeof(string));
        }

        [Fact]
        public void AddRating_ShouldHaveHttpPostAttribute()
        {
            // Arrange & Act
            var methodInfo = typeof(ProductController).GetMethod("AddRating");
            var attribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true).FirstOrDefault() as HttpPostAttribute;

            // Assert
            attribute.ShouldNotBeNull();
            attribute.Template.ShouldBe("{id}/rating");
        }

        [Fact]
        public void GetRatings_ShouldHaveHttpGetAttribute()
        {
            // Arrange & Act
            var methodInfo = typeof(ProductController).GetMethod("GetRatings");
            var attribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true).FirstOrDefault() as HttpGetAttribute;

            // Assert
            attribute.ShouldNotBeNull();
            attribute.Template.ShouldBe("{id}/ratings");
        }

        [Fact]
        public void GetRatings_ShouldAllowAnonymousAccess()
        {
            // Arrange & Act
            var methodInfo = typeof(ProductController).GetMethod("GetRatings");
            var attribute = methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();

            // Assert
            attribute.ShouldNotBeNull("GetRatings should allow anonymous access");
        }

        [Fact]
        public async Task AddRating_WithValidRequest_ReturnsOkObjectResult_WithProductRatingResponse()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new AddRatingRequest { Rating = 5, Comment = "Excellent" };
            var expectedResponse = new ProductRatingResponse
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = "user-123",
                Rating = 5,
                Comment = "Excellent",
                CreatedDate = DateTime.UtcNow
            };

            _mockProductApplication.Setup(x => x.AddRatingAsync(productId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddRating(productId, request);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var value = okResult.Value.ShouldBeOfType<ProductRatingResponse>();
            value.Id.ShouldBe(expectedResponse.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task AddRating_WithOutOfRangeRating_ReturnsBadRequest(int invalidRating)
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new AddRatingRequest { Rating = invalidRating, Comment = "Bad" };

            // Act
            var result = await _controller.AddRating(productId, request);

            // Assert
            var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe("Rating must be between 1 and 5");
        }

        [Fact]
        public async Task AddRating_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            // Act
            var result = await _controller.Add(product: null); // keep existing Add null test; but call AddRating below
            // We want to exercise AddRating null request
            var ratingResult = await _controller.AddRating(productId, null);

            // Assert
            var badRequest = ratingResult.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe("Rating request cannot be null");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddRating_WithInvalidProductId_ReturnsBadRequest(string invalidProductId)
        {
            // Arrange
            var request = new AddRatingRequest { Rating = 4, Comment = "Nice" };

            // Act
            var result = await _controller.AddRating(invalidProductId, request);

            // Assert
            var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe("Product ID is required");
        }

        [Fact]
        public async Task AddRating_WhenUserAlreadyRated_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new AddRatingRequest { Rating = 4, Comment = "Repeat" };
            var errorMessage = "User has already rated this product";
            _mockProductApplication.Setup(x => x.AddRatingAsync(productId, request))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act
            var result = await _controller.AddRating(productId, request);

            // Assert
            var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task AddRating_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new AddRatingRequest { Rating = 3, Comment = "Ok" };
            _mockProductApplication.Setup(x => x.AddRatingAsync(productId, request))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var result = await _controller.AddRating(productId, request);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddRating_WhenUnhandledException_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var request = new AddRatingRequest { Rating = 5, Comment = "Err" };
            var errorMessage = "backend error";
            _mockProductApplication.Setup(x => x.AddRatingAsync(productId, request))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.AddRating(productId, request);

            // Assert
            var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task GetRatings_WithValidProductId_ReturnsOkObjectResult_WithListOfRatings()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var ratings = new List<ProductRatingResponse>
            {
                new ProductRatingResponse { Id = Guid.NewGuid().ToString(), ProductId = productId, UserId = "u1", Rating = 5, Comment = "Great", CreatedDate = DateTime.UtcNow },
                new ProductRatingResponse { Id = Guid.NewGuid().ToString(), ProductId = productId, UserId = "u2", Rating = 4, Comment = "Good", CreatedDate = DateTime.UtcNow }
            };

            _mockProductApplication.Setup(x => x.GetRatingsAsync(productId))
                .ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetRatings(productId);

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            var value = ok.Value.ShouldBeAssignableTo<IEnumerable<ProductRatingResponse>>();
            value.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetRatings_WithNoRatings_ReturnsOkWithEmptyList()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var ratings = new List<ProductRatingResponse>();
            _mockProductApplication.Setup(x => x.GetRatingsAsync(productId))
                .ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetRatings(productId);

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            var value = ok.Value.ShouldBeAssignableTo<IEnumerable<ProductRatingResponse>>();
            value.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetRatings_WithInvalidProductId_ReturnsBadRequest(string invalidProductId)
        {
            // Act
            var result = await _controller.GetRatings(invalidProductId);

            // Assert
            var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
            badRequest.Value.ShouldBe("Product ID is required");
        }

        [Fact]
        public async Task GetRatings_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            _mockProductApplication.Setup(x => x.GetRatingsAsync(productId))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var result = await _controller.GetRatings(productId);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        // test-only stubs are intentionally NOT added here to avoid conflicts; keep tests reflection-based to compile without domain types.
    }
}

