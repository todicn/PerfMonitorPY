namespace ProductBackend.Models;

/// <summary>
/// Configuration model for test products loaded from appsettings.json
/// </summary>
public class ProductConfiguration
{
    /// <summary>
    /// Gets or sets the name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the category of the product
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stock quantity of the product
    /// </summary>
    public int Stock { get; set; }
} 