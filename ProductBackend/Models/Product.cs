namespace ProductBackend.Models;

/// <summary>
/// Represents a product in the system
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Category of the product
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Quantity in stock
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Date when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}