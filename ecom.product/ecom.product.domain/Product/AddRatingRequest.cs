namespace ecom.product.domain.Product
{
	public class AddRatingRequest
	{
		// test-only simple DTO
		public int Rating { get; set; }
		public string Comment { get; set; } = string.Empty;
	}
}
