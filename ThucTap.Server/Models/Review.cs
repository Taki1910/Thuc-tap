namespace ThucTap.Server.Models;

public sealed class Review
{
    public required string Id { get; set; }
    public string ProductId { get; set; } = "";
    public required string Author { get; set; }
    public required int Rating { get; set; }
    public required DateOnly Date { get; set; }
    public required string Comment { get; set; }
    public Product Product { get; set; } = null!;
}
