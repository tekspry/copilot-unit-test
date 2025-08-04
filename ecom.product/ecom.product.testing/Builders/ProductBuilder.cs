using ecom.product.domain.Product;

namespace ecom.product.testing.Builders
{
    public class ProductBuilder
    {
        private readonly Product _product;

        public ProductBuilder()
        {
            _product = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                Name = "Default Product",
                Price = 99,
                Seller = "Default Seller",
                AvailableSince = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ShortDescription = "Default Short Description",
                ImageUrl = "http://default.com/image.jpg",
                Quantity = 1,
                ProductDescription = "Default Product Description"
            };
        }

        public ProductBuilder WithId(string id)
        {
            _product.ProductId = id;
            return this;
        }

        public ProductBuilder WithName(string name)
        {
            _product.Name = name;
            return this;
        }

        public ProductBuilder WithPrice(int price)
        {
            _product.Price = price;
            return this;
        }

        public ProductBuilder WithSeller(string seller)
        {
            _product.Seller = seller;
            return this;
        }

        public ProductBuilder WithAvailableSince(string date)
        {
            _product.AvailableSince = date;
            return this;
        }

        public ProductBuilder WithShortDescription(string description)
        {
            _product.ShortDescription = description;
            return this;
        }

        public ProductBuilder WithImageUrl(string url)
        {
            _product.ImageUrl = url;
            return this;
        }

        public ProductBuilder WithQuantity(int quantity)
        {
            _product.Quantity = quantity;
            return this;
        }

        public ProductBuilder WithProductDescription(string description)
        {
            _product.ProductDescription = description;
            return this;
        }

        public Product Build()
        {
            return _product;
        }

        public static ProductBuilder Create()
        {
            return new ProductBuilder();
        }
    }
}
