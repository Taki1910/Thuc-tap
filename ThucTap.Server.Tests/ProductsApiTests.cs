using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThucTap.Server.Controllers;
using ThucTap.Server.Data;
using ThucTap.Server.Services;

namespace ThucTap.Server.Tests;

public sealed class ProductsApiTests
{
    [Fact]
    public async Task Product_and_review_endpoints_validate_and_persist_records()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .ConfigureServices(services =>
            {
                services.AddDbContext<StoreDbContext>(options => options.UseSqlite(connection));
                services.AddScoped<IProductService, ProductService>();
                services.AddControllers().AddApplicationPart(typeof(ProductsController).Assembly);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }))
            .StartAsync();

        using var client = host.GetTestClient();
        await using (var scope = host.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            await db.Database.EnsureCreatedAsync();
            db.Products.AddRange(SampleProducts.All);
            await db.SaveChangesAsync();
        }

        var list = await client.GetFromJsonAsync<ProductSummary[]>("/api/products");
        Assert.NotNull(list);
        Assert.Equal(4, list.Length);
        var filtered = await client.GetFromJsonAsync<ProductSummary[]>("/api/products?search=CANVAS");
        var canvas = Assert.Single(filtered!);
        Assert.Equal("canvas-tote", canvas.Id);
        Assert.Equal(2, canvas.ReviewCount);
        Assert.Equal(4.5m, canvas.Rating);

        var detail = await client.GetFromJsonAsync<ProductDetail>("/api/products/canvas-tote");
        Assert.NotNull(detail);
        Assert.Equal(canvas.ReviewCount, detail.ReviewCount);
        Assert.Equal(canvas.Rating, detail.Rating);
        Assert.Equal(2, detail.Reviews.Length);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/products/missing")).StatusCode);

        var invalidProduct = await client.PostAsJsonAsync("/api/products", new
        {
            name = "", category = "Office", price = -1, imageUrl = "https://example.test/item.png", description = "Bad"
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidProduct.StatusCode);

        var missingPrice = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Missing price", category = "Office", imageUrl = "https://example.test/item.png", description = "Price is required"
        });
        Assert.Equal(HttpStatusCode.BadRequest, missingPrice.StatusCode);

        var createdResponse = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Test item", category = "Office", price = 1000, imageUrl = "https://example.test/item.png", description = "A test item"
        });
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await createdResponse.Content.ReadFromJsonAsync<ProductDetail>();
        Assert.NotNull(created);

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync($"/api/products/{created.Id}/reviews", new
        {
            author = "Reviewer", rating = 6, comment = "Out of range"
        })).StatusCode);

        var reviewResponse = await client.PostAsJsonAsync($"/api/products/{created.Id}/reviews", new
        {
            author = "Reviewer", rating = 4, comment = "Useful item"
        });
        Assert.Equal(HttpStatusCode.Created, reviewResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/products/{created.Id}/reviews")).StatusCode);

        var update = await client.PutAsJsonAsync($"/api/products/{created.Id}", new
        {
            name = "Updated item", category = "Office", price = 1200, imageUrl = "https://example.test/item.png", description = "Updated"
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var updated = await update.Content.ReadFromJsonAsync<ProductDetail>();
        Assert.NotNull(updated);
        Assert.Equal("Updated item", updated.Name);
        Assert.Equal(1, updated.ReviewCount);
        Assert.Equal(4m, updated.Rating);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/products/missing", new
        {
            name = "x", category = "x", price = 0, imageUrl = "x", description = "x"
        })).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/products/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/products/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/products/{created.Id}/reviews")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/products/missing/reviews", new
        {
            author = "Reviewer", rating = 4, comment = "Missing product"
        })).StatusCode);
    }

    private sealed record ProductSummary(string Id, decimal Rating, int ReviewCount);
    private sealed record ProductDetail(string Id, string Name, decimal Rating, int ReviewCount, ReviewResponse[] Reviews);
    private sealed record ReviewResponse(string Id, string Author, int Rating, string Date, string Comment);

}
