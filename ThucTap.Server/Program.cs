using ThucTap.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Configure ConnectionStrings:DefaultConnection before starting the API. For local development, set it with " +
        "'dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<connection-string>\" --project ThucTap.Server'.");
builder.Services.AddDbContext<StoreDbContext>(options => StoreDbContext.Configure(options, connectionString));
builder.Services.AddScoped<ThucTap.Server.Services.IProductService, ThucTap.Server.Services.ProductService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentFrontend", policy => policy
        .WithOrigins(builder.Configuration.GetSection("Frontend:Origins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentFrontend");
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

public partial class Program;
