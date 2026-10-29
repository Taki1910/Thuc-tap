# Mộc Market — React + ASP.NET Core + SQL Server

Product catalog and reviews MVP built on the existing React/Vite and ASP.NET Core 10 projects. Products and reviews are stored in SQL Server through EF Core; sample records are inserted once by the migration seed hook.

## Prerequisites

- .NET SDK 10
- Node.js 20.19+ or 22.12+
- SQL Server or LocalDB, with permission to create/update the `ThucTap` database

The API reads `ConnectionStrings:DefaultConnection`. No credentials are stored in this repository.

For local development, save the connection string in User Secrets:

```powershell
dotnet user-secrets init --project ThucTap.Server
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=ThucTap;Trusted_Connection=True;TrustServerCertificate=True" --project ThucTap.Server
```

For SQL Server Express, replace the server name, for example `Server=localhost\SQLEXPRESS;Database=ThucTap;Trusted_Connection=True;TrustServerCertificate=True`. In hosted environments, use the `ConnectionStrings__DefaultConnection` environment variable or the platform's secret store.

## Create or update the database

Restore the checked-in EF tool, then apply migrations:

```powershell
dotnet tool restore
dotnet ef database update --project ThucTap.Server --startup-project ThucTap.Server
```

The database must be reachable when running `database update`. EF Core's repeatable seed hook inserts each known sample product only if its ID is not already present. The API does not auto-run schema changes at startup.

When changing the data model, create a migration and then apply it:

```powershell
dotnet ef migrations add DescribeYourChange --project ThucTap.Server --output-dir Data/Migrations
dotnet ef database update --project ThucTap.Server --startup-project ThucTap.Server
```

## Run the application

After configuring a reachable SQL Server and applying migrations, start the API and existing Vite SPA Proxy from the repository root:

```powershell
dotnet run --project ThucTap.Server --launch-profile https
```

Open the Vite URL printed in the terminal (normally `https://localhost:63341`). For a separate frontend dev server, set `VITE_API_PROXY_TARGET` to the API address as described in `vite.config.ts`.

## API

JSON uses camelCase. Product lists are capped at 100 results. Product detail and the reviews subresource return the 100 newest reviews; `reviewCount` and `rating` aggregate all saved reviews.

| Method | Path | Result |
| --- | --- | --- |
| GET | `/api/products` | Product summaries |
| GET | `/api/products?search=canvas` | Search by name, category, or description |
| GET | `/api/products/{id}` | Product detail with reviews |
| POST | `/api/products` | Create a product (`201`) |
| PUT | `/api/products/{id}` | Replace editable product fields |
| DELETE | `/api/products/{id}` | Delete a product and its reviews (`204`) |
| GET | `/api/products/{id}/reviews` | Product's newest reviews |
| POST | `/api/products/{id}/reviews` | Add a review (`201`) |

Create/update product body:

```json
{
  "name": "Túi vải",
  "category": "Phụ kiện",
  "price": 195000,
  "imageUrl": "https://example.com/tui-vai.jpg",
  "description": "Túi canvas dùng hàng ngày."
}
```

Add review body:

```json
{
  "author": "Minh Anh",
  "rating": 5,
  "comment": "Đường may chắc chắn, dùng rất tiện."
}
```

Required fields and maximum lengths are validated by the API. Price must be non-negative; rating must be from 1 to 5. Invalid input returns `400`, unknown IDs return `404`. The frontend includes product create/edit/delete and add-review forms with server validation feedback.

## Build and tests

From the repository root:

```powershell
dotnet build ThucTap.Server/ThucTap.Server.csproj
dotnet test ThucTap.Server.Tests/ThucTap.Server.Tests.csproj
Push-Location thuctap.client
npm ci
npm run build
npm run lint
Pop-Location
```

The API integration test host uses a temporary SQLite database to exercise list, search, detail, create, update, delete, review endpoints, invalid input, and missing IDs without requiring SQL Server. It does not verify SQL Server migration application or restart persistence. Run `dotnet ef database update` against your configured SQL Server to verify those locally.

## Main files and limitations

- Backend: `ThucTap.Server/Data/StoreDbContext.cs`, `Data/Migrations/`, `Services/ProductService.cs`, `Controllers/ProductsController.cs`, and `DTOs/`
- API tests: `ThucTap.Server.Tests/ProductsApiTests.cs`
- Frontend: `src/App.tsx`, `src/ProductForm.tsx`, `src/api.ts`, and `src/types.ts`

This MVP has no authentication or authorization; any client that can reach the API can create, edit, or delete catalog data. Lists and review displays are bounded but do not yet expose pagination. Product images use URLs and are not uploaded or stored by the server.
