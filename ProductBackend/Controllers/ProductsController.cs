using Microsoft.AspNetCore.Mvc;
using ProductBackend.Models;
using ProductBackend.Services;

namespace ProductBackend.Controllers;

/// <summary>
/// Controller for managing product operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of all products</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        return Ok(_productService.GetAllProducts());
    }

    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product if found</returns>
    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }
        return Ok(product);
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>Products in the specified category</returns>
    [HttpGet("category/{category}")]
    public ActionResult<IEnumerable<Product>> GetProductsByCategory(string category)
    {
        var products = _productService.GetProductsByCategory(category);
        return Ok(products);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public ActionResult<Product> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return BadRequest("Product name is required.");
        }

        if (product.Price < 0)
        {
            return BadRequest("Product price cannot be negative.");
        }

        var createdProduct = _productService.AddProduct(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, [FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return BadRequest("Product name is required.");
        }

        if (product.Price < 0)
        {
            return BadRequest("Product price cannot be negative.");
        }

        var updatedProduct = _productService.UpdateProduct(id, product);
        if (updatedProduct == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }

        return Ok(updatedProduct);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var deleted = _productService.DeleteProduct(id);
        if (!deleted)
        {
            return NotFound($"Product with ID {id} not found.");
        }

        return NoContent();
    }
}