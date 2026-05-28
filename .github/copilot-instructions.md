# Copilot Instructions for Modernization Hackathon

## Project Context

This is a **legacy .NET 7 REST API** (Inventory Management) that participants must modernize to **.NET 10** using AI-powered agents, skills, MCP servers, and custom prompts.

## Architecture Overview

- **Framework**: ASP.NET Core 7.0 (EOL — out of support)
- **Pattern**: MVC Controllers with Startup.cs + Program.Main()
- **Persistence**: JSON file-based (Newtonsoft.Json) — no database
- **Serialization**: Newtonsoft.Json (legacy)
- **API Structure**: RESTful with `/api/products` and `/api/categories`

## Key Legacy Patterns to Modernize

1. **Hosting model**: Uses `IHostBuilder` + `Startup.cs` pattern instead of modern minimal hosting
2. **Serialization**: Uses `Newtonsoft.Json` instead of `System.Text.Json`
3. **Target framework**: `net7.0` (EOL) → should target `net10.0`
4. **OpenAPI**: Uses Swashbuckle instead of built-in `Microsoft.AspNetCore.OpenApi`
5. **Endpoint routing**: Uses `app.UseEndpoints()` legacy pattern
6. **Async patterns**: Repository uses `Task.FromResult` wrapping synchronous code
7. **Concurrency**: Uses `lock` instead of modern concurrent patterns
8. **Validation**: Manual validation in controllers instead of using FluentValidation or Data Annotations
9. **Error handling**: No global exception handling middleware
10. **No tests**: Zero test coverage

## File Structure

```
src/LegacyInventoryApi/
├── Program.cs              # Legacy IHostBuilder pattern
├── Startup.cs              # Legacy Startup class
├── Controllers/
│   ├── ProductsController.cs
│   └── CategoriesController.cs
├── Models/
│   ├── Product.cs
│   ├── Category.cs
│   └── ApiResponse.cs
├── Repositories/
│   ├── IProductRepository.cs
│   ├── ICategoryRepository.cs
│   ├── JsonProductRepository.cs
│   └── JsonCategoryRepository.cs
└── Data/                   # JSON files generated at runtime
```

## API Endpoints

### Products
- `GET /api/products` — List products (supports `?page=`, `?pageSize=`, `?category=`, `?activeOnly=`)
- `GET /api/products/{id}` — Get product by ID
- `GET /api/products/search?q=` — Search products
- `POST /api/products` — Create a product
- `PUT /api/products/{id}` — Update a product
- `DELETE /api/products/{id}` — Delete a product

### Categories
- `GET /api/categories` — List categories
- `GET /api/categories/{id}` — Get category by ID
- `POST /api/categories` — Create a category
- `PUT /api/categories/{id}` — Update a category
- `DELETE /api/categories/{id}` — Delete a category

## Modernization Goals

When assisting with modernization, ensure:
- Target framework is upgraded to `net10.0`
- Minimal hosting (top-level `Program.cs`, no `Startup.cs`)
- `System.Text.Json` replaces `Newtonsoft.Json`
- Built-in OpenAPI replaces Swashbuckle
- Modern C# features (file-scoped namespaces, primary constructors, collection expressions)
- All existing API behavior and endpoints are preserved
- JSON persistence continues to work with same data format
- Unit tests are added for repository and controller logic
