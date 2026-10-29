namespace ThucTap.Server.DTOs;

public sealed record ProductDetailDto(
    string Id,
    string Name,
    string Category,
    decimal Price,
    string ImageUrl,
    decimal Rating,
    int ReviewCount,
    string Description,
    IReadOnlyList<ReviewDto> Reviews)
    : ProductSummaryDto(Id, Name, Category, Price, ImageUrl, Rating, ReviewCount);
