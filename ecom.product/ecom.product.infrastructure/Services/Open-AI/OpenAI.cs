using ecom.product.domain.Product;
using OpenAI;
using OpenAI.Models;
using System.Text;

namespace ecom.product.infrastructure.Services.Open_AI
{
    public class ProductOpenAI : IProductOpenAI
    {
        public async Task<Product> GenerateProductDescription(Product product)
        {
            var outProductDescription = new StringBuilder();
            
            try
            {   
                var prompt = $"You are a content writer. As a content writer, write a 500 words description for product sold on ecommerce website. " +
                    $"Short description of  the product is '{product.ShortDescription}'. Name of the product is '{product.Name}', It is sold by '{product.Seller}'. Total price of the product is {product.Price}. " +
                    $"Format the response to define each feature as separate paragraph";


                // Authentication settings
                var apiKey = "";

                // OpenAI API completion parameters
                int maxTokens = 2000;
                double temperature = 0.5;
                double presencePenalty = 0.1;
                double frequencyPenalty = 0.1;

                // Instantiate OpenAIClient
                var openAIClient = new OpenAIClient(apiKey);

                
                // Stream completion results and write to file
                await openAIClient.CompletionsEndpoint.StreamCompletionAsync(result =>
                {
                    foreach (var token in result.Completions)
                    {
                        outProductDescription.Append(token.ToString());
                    }
                }, prompt, maxTokens: maxTokens, temperature: temperature, presencePenalty: presencePenalty, frequencyPenalty: frequencyPenalty, model: Model.Davinci);

                product.ProductDescription = outProductDescription.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing analysis: {ex.Message}");
            }

            return product;
        }
    }
}
