using Microsoft.AspNetCore.Mvc;
using ThucTap.Server.DTOs;
using ThucTap.Server.Services;

namespace ThucTap.Server.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductSummaryDto>>> GetProducts(
        [FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await products.GetProductsAsync(search, cancellationToken));

    [HttpGet("{id}")]
    [ProducesResponseType<ProductDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetProductById(string id, CancellationToken cancellationToken)
    {
        var product = await products.GetProductByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType<ProductDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDetailDto>> CreateProduct(
        ProductWriteRequest request, CancellationToken cancellationToken)
    {
        var product = await products.CreateProductAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [ProducesResponseType<ProductDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> UpdateProduct(
        string id, ProductWriteRequest request, CancellationToken cancellationToken)
    {
        var product = await products.UpdateProductAsync(id, request, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(string id, CancellationToken cancellationToken) =>
        await products.DeleteProductAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpGet("{id}/reviews")]
    [ProducesResponseType<IReadOnlyList<ReviewDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetReviews(
        string id, CancellationToken cancellationToken)
    {
        var reviews = await products.GetReviewsAsync(id, cancellationToken);
        return reviews is null ? NotFound() : Ok(reviews);
    }

    [HttpPost("{id}/reviews")]
    [ProducesResponseType<ReviewDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> AddReview(
        string id, ReviewWriteRequest request, CancellationToken cancellationToken)
    {
        var review = await products.AddReviewAsync(id, request, cancellationToken);
        return review is null ? NotFound() : CreatedAtAction(nameof(GetReviews), new { id }, review);
    }
}
