using ThucTap.Server.DTOs;

namespace ThucTap.Server.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<ProductDetailDto?> GetProductByIdAsync(string id, CancellationToken cancellationToken);
    Task<ProductDetailDto> CreateProductAsync(ProductWriteRequest request, CancellationToken cancellationToken);
    Task<ProductDetailDto?> UpdateProductAsync(string id, ProductWriteRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReviewDto>?> GetReviewsAsync(string productId, CancellationToken cancellationToken);
    Task<ReviewDto?> AddReviewAsync(string productId, ReviewWriteRequest request, CancellationToken cancellationToken);
}
