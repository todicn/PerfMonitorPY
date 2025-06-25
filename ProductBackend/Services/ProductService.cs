using Microsoft.Extensions.Options;
using ProductBackend.Models;

namespace ProductBackend.Services;

/// <summary>
/// Service class for managing product operations
/// </summary>
public class ProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    /// <summary>
    /// Initializes a new instance of the ProductService class
    /// </summary>
    /// <param name="testProductsOptions">Configuration options for test products</param>
    public ProductService(IOptions<List<ProductConfiguration>> testProductsOptions)
    {
        // Load test products from configuration
        var testProducts = testProductsOptions.Value ?? new List<ProductConfiguration>();
        
        foreach (var testProduct in testProducts)
        {
            _products.Add(new Product
            {
                Id = _nextId++,
                Name = testProduct.Name,
                Description = testProduct.Description,
                Price = testProduct.Price,
                Category = testProduct.Category,
                Stock = testProduct.Stock,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of all products</returns>
    public IEnumerable<Product> GetAllProducts()
    {
        return _products.AsReadOnly();
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product if found, null otherwise</returns>
    public Product? GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    /// <summary>
    /// Adds a new product
    /// </summary>
    /// <param name="product">Product to add</param>
    /// <returns>The added product with assigned ID</returns>
    public Product AddProduct(Product product)
    {
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        return product;
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updatedProduct">Updated product data</param>
    /// <returns>Updated product if found, null otherwise</returns>
    public Product? UpdateProduct(int id, Product updatedProduct)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        if (existingProduct == null)
            return null;

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Category = updatedProduct.Category;
        existingProduct.Stock = updatedProduct.Stock;

        return existingProduct;
    }

    /// <summary>
    /// Deletes a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if deleted, false if not found</returns>
    public bool DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return false;

        _products.Remove(product);
        return true;
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>Products in the specified category</returns>
    public IEnumerable<Product> GetProductsByCategory(string category)
    {
        return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }
}