using cosmosdb_seeding.ApiService.Data;
using cosmosdb_seeding.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cosmosdb_seeding.ApiService.Controllers;

/// <summary>
/// API controller for managing seed items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SeedItemsController : ControllerBase
{
    private readonly SeedingDbContext _context;
    private readonly ILogger<SeedItemsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedItemsController"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public SeedItemsController(SeedingDbContext context, ILogger<SeedItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets all seed items.
    /// </summary>
    /// <returns>A list of all seed items.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SeedItem>>> GetSeedItems()
    {
        try
        {
            _logger.LogInformation("Retrieving all seed items");
            var items = await _context.SeedItems
                .Where(item => item.IsActive)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} seed items", items.Count);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seed items");
            return StatusCode(500, "An error occurred while retrieving seed items");
        }
    }

    /// <summary>
    /// Gets a specific seed item by ID.
    /// </summary>
    /// <param name="id">The ID of the seed item.</param>
    /// <returns>The seed item if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SeedItem>> GetSeedItem(string id)
    {
        try
        {
            _logger.LogInformation("Retrieving seed item with ID: {Id}", id);
            
            var item = await _context.SeedItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                _logger.LogWarning("Seed item with ID {Id} not found", id);
                return NotFound($"Seed item with ID '{id}' not found");
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seed item with ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the seed item");
        }
    }

    /// <summary>
    /// Gets seed items by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>A list of seed items in the specified category.</returns>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<SeedItem>>> GetSeedItemsByCategory(string category)
    {
        try
        {
            _logger.LogInformation("Retrieving seed items for category: {Category}", category);
            
            var items = await _context.SeedItems
                .Where(item => item.IsActive && item.Category.ToLower() == category.ToLower())
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} seed items for category {Category}", items.Count, category);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seed items for category: {Category}", category);
            return StatusCode(500, "An error occurred while retrieving seed items by category");
        }
    }

    /// <summary>
    /// Creates a new seed item.
    /// </summary>
    /// <param name="item">The seed item to create.</param>
    /// <returns>The created seed item.</returns>
    [HttpPost]
    public async Task<ActionResult<SeedItem>> CreateSeedItem(SeedItem item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure ID is set
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            item.CreatedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation("Creating new seed item: {Name} (ID: {Id})", item.Name, item.Id);

            _context.SeedItems.Add(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created seed item with ID: {Id}", item.Id);
            return CreatedAtAction(nameof(GetSeedItem), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating seed item: {Name}", item?.Name);
            return StatusCode(500, "An error occurred while creating the seed item");
        }
    }

    /// <summary>
    /// Gets the count of items in each category.
    /// </summary>
    /// <returns>A dictionary with category counts.</returns>
    [HttpGet("stats/categories")]
    public async Task<ActionResult<Dictionary<string, int>>> GetCategoryStats()
    {
        try
        {
            _logger.LogInformation("Retrieving category statistics");
            
            var stats = await _context.SeedItems
                .Where(item => item.IsActive)
                .GroupBy(item => item.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            _logger.LogInformation("Retrieved statistics for {Count} categories", stats.Count);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category statistics");
            return StatusCode(500, "An error occurred while retrieving category statistics");
        }
    }
}
