namespace ThucTap.Server.DTOs;

public sealed record ReviewDto(
    string Id,
    string Author,
    int Rating,
    DateOnly Date,
    string Comment);
