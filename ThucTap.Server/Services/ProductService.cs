using Microsoft.EntityFrameworkCore;
using ThucTap.Server.Data;
using ThucTap.Server.DTOs;
using ThucTap.Server.Models;

namespace ThucTap.Server.Services;

public sealed class ProductService(StoreDbContext db) : IProductService
{
    public async Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var products = db.Products.AsNoTracking();
        var term = searchTerm?.Trim().ToLowerInvariant();

        if (!string.IsNullOrEmpty(term))
            products = products.Where(product => product.Name.ToLower().Contains(term) ||
                product.Category.ToLower().Contains(term) || product.Description.ToLower().Contains(term));

        return await products
            .OrderBy(product => product.Name)
            .Take(100)
            .Select(product => new ProductSummaryDto(
                product.Id,
                product.Name,
                product.Category,
                product.Price,
                product.ImageUrl,
                Math.Round(product.Reviews.Select(review => (decimal?)review.Rating).Average() ?? 0m, 1),
                product.Reviews.Count))
            .ToArrayAsync(cancellationToken);
    }

    public Task<ProductDetailDto?> GetProductByIdAsync(string id, CancellationToken cancellationToken) =>
        db.Products.AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductDetailDto(
                product.Id,
                product.Name,
                product.Category,
                product.Price,
                product.ImageUrl,
                Math.Round(product.Reviews.Select(review => (decimal?)review.Rating).Average() ?? 0m, 1),
                product.Reviews.Count,
                product.Description,
                product.Reviews.OrderByDescending(review => review.Date).ThenBy(review => review.Id)
                    .Take(100)
                    .Select(review => new ReviewDto(review.Id, review.Author, review.Rating, review.Date, review.Comment))
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<ProductDetailDto> CreateProductAsync(ProductWriteRequest request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Price = request.Price!.Value,
            ImageUrl = request.ImageUrl.Trim(),
            Description = request.Description.Trim()
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return new ProductDetailDto(product.Id, product.Name, product.Category, product.Price,
            product.ImageUrl, 0m, 0, product.Description, []);
    }

    public async Task<ProductDetailDto?> UpdateProductAsync(string id, ProductWriteRequest request, CancellationToken cancellationToken)
    {
        var product = await db.Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
        if (product is null) return null;

        product.Name = request.Name.Trim();
        product.Category = request.Category.Trim();
        product.Price = request.Price!.Value;
        product.ImageUrl = request.ImageUrl.Trim();
        product.Description = request.Description.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return await GetProductByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken)
    {
        var product = await db.Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
        if (product is null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ReviewDto>?> GetReviewsAsync(string productId, CancellationToken cancellationToken)
    {
        var exists = await db.Products.AnyAsync(product => product.Id == productId, cancellationToken);
        if (!exists) return null;

        return await db.Reviews.AsNoTracking()
            .Where(review => review.ProductId == productId)
            .OrderByDescending(review => review.Date)
            .ThenBy(review => review.Id)
            .Take(100)
            .Select(review => new ReviewDto(review.Id, review.Author, review.Rating, review.Date, review.Comment))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ReviewDto?> AddReviewAsync(string productId, ReviewWriteRequest request, CancellationToken cancellationToken)
    {
        var exists = await db.Products.AnyAsync(product => product.Id == productId, cancellationToken);
        if (!exists) return null;

        var review = new Review
        {
            Id = Guid.NewGuid().ToString("N"),
            ProductId = productId,
            Author = request.Author.Trim(),
            Rating = request.Rating,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Comment = request.Comment.Trim()
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);
        return new ReviewDto(review.Id, review.Author, review.Rating, review.Date, review.Comment);
    }
}
