# Acceptance Criteria

## Modernization Hackathon — Acceptance Criteria

Participants must modernize the Legacy Inventory API **in a single prompt** (or a single agent invocation) and meet ALL of the following criteria to pass.

---

### ✅ AC1: Target Framework Upgrade

- [ ] Project targets `net10.0`
- [ ] All NuGet packages are updated to their latest stable versions compatible with .NET 10
- [ ] Project builds successfully with zero errors and zero warnings (excluding `nullable` informational messages)

### ✅ AC2: Modern Hosting Pattern

- [ ] `Startup.cs` is removed
- [ ] `Program.cs` uses minimal hosting (top-level statements or a concise `Program` class)
- [ ] Dependency injection is configured inline in `Program.cs`
- [ ] Middleware pipeline is configured using the modern `WebApplication` builder pattern

### ✅ AC3: System.Text.Json Migration

- [ ] `Newtonsoft.Json` NuGet package is removed from the project
- [ ] `Microsoft.AspNetCore.Mvc.NewtonsoftJson` package is removed
- [ ] All serialization uses `System.Text.Json`
- [ ] JSON property naming (camelCase) is preserved
- [ ] Null value handling behavior is preserved
- [ ] Existing JSON data files remain compatible (can be read after migration)

### ✅ AC4: OpenAPI Modernization

- [ ] `Swashbuckle.AspNetCore` package is removed
- [ ] Built-in `Microsoft.AspNetCore.OpenApi` is used
- [ ] OpenAPI document is accessible at runtime (e.g., `/openapi/v1.json`)

### ✅ AC5: Modern C# Language Features

- [ ] File-scoped namespaces are used throughout
- [ ] Primary constructors are used where appropriate (controllers, services)
- [ ] Collection expressions (`[]`) are used where appropriate
- [ ] `record` types are used for request/response DTOs where appropriate
- [ ] Nullable reference types are properly annotated

### ✅ AC6: API Behavioral Compatibility

- [ ] All existing endpoints return identical response shapes
- [ ] HTTP status codes are preserved (200, 201, 400, 404)
- [ ] Pagination, filtering, and search behavior is unchanged
- [ ] Seed data is preserved and auto-generated on first run
- [ ] `CreatedAtAction` still returns proper `Location` headers

### ✅ AC7: Improved Async & Concurrency

- [ ] Repository implementations use proper async I/O (`File.ReadAllTextAsync`, `File.WriteAllTextAsync`)
- [ ] Thread-safety is maintained (e.g., `SemaphoreSlim` or `Channel` instead of `lock`)

### ✅ AC8: Error Handling & Validation

- [ ] Global exception handling middleware is added
- [ ] Input validation uses Data Annotations or a validation library
- [ ] Validation errors return consistent 400 responses

### ✅ AC9: Test Coverage

- [ ] A test project is created targeting `net10.0`
- [ ] Unit tests cover repository CRUD operations
- [ ] Unit tests cover controller logic (happy path + error cases)
- [ ] All tests pass with `dotnet test`

### ✅ AC10: Code Quality

- [ ] No `// TODO` or `// HACK` comments remain
- [ ] No unused `using` statements
- [ ] Consistent code formatting
- [ ] Solution builds and runs with `dotnet run`

---

## Scoring Rubric

| Criteria | Points |
|----------|--------|
| AC1: Framework Upgrade | 10 |
| AC2: Modern Hosting | 10 |
| AC3: System.Text.Json | 15 |
| AC4: OpenAPI | 10 |
| AC5: Modern C# | 10 |
| AC6: API Compatibility | 20 |
| AC7: Async & Concurrency | 10 |
| AC8: Error Handling | 5 |
| AC9: Test Coverage | 5 |
| AC10: Code Quality | 5 |
| **Total** | **100** |

## Bonus Points

| Bonus | Points |
|-------|--------|
| Completed in a single prompt without manual intervention | +10 |
| Agent also adds Docker support | +5 |
| Agent adds health check endpoint (`/healthz`) | +5 |
| Agent adds structured logging (Serilog or similar) | +5 |
| Agent adds API versioning | +5 |
| **Max Bonus** | **+30** |

---

## How to Validate

```bash
# 1. Build must succeed
dotnet build --no-restore

# 2. Tests must pass
dotnet test --no-build

# 3. App must start and respond
dotnet run --project src/LegacyInventoryApi
# In another terminal:
curl http://localhost:5150/api/products
curl http://localhost:5150/api/categories

# 4. Verify OpenAPI
curl http://localhost:5150/openapi/v1.json

# 5. Verify no Newtonsoft.Json references
# (should return no results)
grep -r "Newtonsoft" src/
```
