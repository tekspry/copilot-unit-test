# XML Documentation Comments Template Guide

This guide provides templates and best practices for writing comprehensive XML documentation comments in C# code, following Microsoft's documentation standards.

## Basic Template Structure

```csharp
/// <summary>
/// Brief description of the method/class/property
/// </summary>
/// <param name="parameterName">Description of the parameter</param>
/// <returns>Description of the return value</returns>
/// <remarks>
/// Detailed information about the method/class/property
/// </remarks>
/// <exception cref="ExceptionType">Description of when this exception is thrown</exception>
```

## Comprehensive Example

```csharp
/// <summary>
/// [Action verb] [subject] [details]
/// Example: Updates an existing product in the cache store.
/// </summary>
/// <param name="parameterName">Description including type and purpose</param>
/// <returns>Description of what is returned, including type if relevant</returns>
/// <remarks>
/// Detailed implementation notes including:
/// - How the method works
/// - Important considerations
/// 
/// Typical usage:
/// <code>
/// // Example code showing how to use the method
/// var instance = new ClassName();
/// var result = await instance.MethodName(parameter);
/// </code>
/// 
/// Called by:
/// - List calling methods/components with their purposes
/// - Example: Controller.Method() - HTTP endpoint purpose
/// 
/// Dependencies:
/// - List required services, components, or methods
/// - Include version requirements if applicable
/// </remarks>
/// <exception cref="ExceptionType">Clear description of when/why this exception occurs</exception>
```

## Best Practices

1. **Summary Section**
   - Start with an action verb
   - Be concise but descriptive
   - Focus on WHAT the code does, not HOW

2. **Parameters**
   - Document all parameters
   - Include type information if not obvious
   - Explain valid ranges or constraints

3. **Returns**
   - Specify the type being returned
   - Explain possible return values
   - Note any special conditions

4. **Remarks**
   - Include implementation details
   - Provide usage examples
   - List dependencies
   - Document calling methods/components
   - Add business logic context

5. **Exceptions**
   - Document all possible exceptions
   - Explain conditions that trigger them
   - Include mitigation steps if applicable

## Real-World Example

```csharp
/// <summary>
/// Updates an existing product in the cache store.
/// </summary>
/// <param name="product">The product object containing updated information.</param>
/// <returns>Returns the price of the updated product.</returns>
/// <remarks>
/// This method updates a product in the distributed cache using Dapr state management.
/// If the product exists, it will be replaced with the updated version.
/// 
/// Typical usage:
/// <code>
/// var productRepo = new ProductRepository(daprClient);
/// var product = new Product 
/// { 
///     ProductId = 1, 
///     Name = "Updated Product",
///     Price = 29.99
/// };
/// int updatedPrice = await productRepo.UpdateProduct(product);
/// </code>
/// 
/// Called by:
/// - ProductController.UpdateProduct(Product product) - HTTP PUT endpoint
/// - ProductService.UpdateProductDetails(Product product) - Business logic layer
/// 
/// Dependencies:
/// - DaprClient for state management
/// - SaveProductListToCacheStore() internal method for persisting changes
/// </remarks>
/// <exception cref="InvalidOperationException">Thrown when the cache store is not accessible.</exception>
```

## Tips for Writing Good Comments

1. Keep summaries concise and to the point
2. Use complete sentences ending with periods
3. Use third person perspective ("Gets" not "Get")
4. Include code examples for complex operations
5. Document all exceptions that can be thrown
6. List dependencies and requirements
7. Provide context about where and how the code is used
8. Update comments when code changes
