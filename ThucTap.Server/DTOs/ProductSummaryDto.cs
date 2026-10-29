namespace ThucTap.Server.DTOs;

public record ProductSummaryDto(
    string Id,
    string Name,
    string Category,
    decimal Price,
    string ImageUrl,
    decimal Rating,
    int ReviewCount);
