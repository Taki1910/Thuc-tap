using System.ComponentModel.DataAnnotations;

namespace ThucTap.Server.DTOs;

public sealed class ReviewWriteRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Author { get; init; } = "";

    [Range(1, 5)]
    public int Rating { get; init; }

    [Required, StringLength(2000, MinimumLength = 1)]
    public string Comment { get; init; } = "";
}
