using System.ComponentModel.DataAnnotations;

namespace cosmosdb_seeding.ApiService.Models;

/// <summary>
/// Represents a seeding item in the Cosmos DB database.
/// </summary>
public class SeedItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// This serves as both the document ID and partition key.
    /// </summary>
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the name of the seed item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the seed item.
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the seed item.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the item was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets whether the item is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets additional metadata for the item.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
