using System.ComponentModel.DataAnnotations;

namespace ThucTap.Server.DTOs;

public sealed class ProductWriteRequest
{
    [Required, StringLength(160, MinimumLength = 1)]
    public string Name { get; init; } = "";

    [Required, StringLength(80, MinimumLength = 1)]
    public string Category { get; init; } = "";

    [Required, Range(typeof(decimal), "0", "9999999999999999")]
    public decimal? Price { get; init; }

    [Required, StringLength(2048, MinimumLength = 1)]
    public string ImageUrl { get; init; } = "";

    [Required, StringLength(4000, MinimumLength = 1)]
    public string Description { get; init; } = "";
}
