namespace ThucTap.Server.Models;

public sealed class Product
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required string ImageUrl { get; set; }
    public required string Description { get; set; }
    public List<Review> Reviews { get; set; } = [];
}
