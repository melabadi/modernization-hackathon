using LegacyInventoryApi.Models;
using Newtonsoft.Json;

namespace LegacyInventoryApi.Repositories
{
    public class JsonProductRepository : IProductRepository
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private List<Product> _products;

        public JsonProductRepository(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "products.json");
            _products = LoadFromFile();
        }

        private List<Product> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                var seedData = GetSeedData();
                SaveToFile(seedData);
                return seedData;
            }

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
        }

        private void SaveToFile(List<Product> products)
        {
            var json = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        private List<Product> GetSeedData()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse with USB receiver", Price = 29.99m, StockQuantity = 150, CategoryId = 1, Sku = "ELEC-001", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 2, Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard with Cherry MX switches", Price = 89.99m, StockQuantity = 75, CategoryId = 1, Sku = "ELEC-002", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 3, Name = "USB-C Hub", Description = "7-in-1 USB-C hub with HDMI and ethernet", Price = 45.99m, StockQuantity = 200, CategoryId = 1, Sku = "ELEC-003", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 4, Name = "Standing Desk", Description = "Electric height-adjustable standing desk", Price = 499.99m, StockQuantity = 30, CategoryId = 2, Sku = "FURN-001", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 5, Name = "Office Chair", Description = "Ergonomic mesh office chair with lumbar support", Price = 299.99m, StockQuantity = 45, CategoryId = 2, Sku = "FURN-002", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 6, Name = "Monitor Arm", Description = "Dual monitor arm with gas spring", Price = 79.99m, StockQuantity = 60, CategoryId = 2, Sku = "FURN-003", IsActive = false, CreatedAt = DateTime.UtcNow },
                new Product { Id = 7, Name = "Notebook A5", Description = "Premium hardcover A5 dotted notebook", Price = 12.99m, StockQuantity = 500, CategoryId = 3, Sku = "STAT-001", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Id = 8, Name = "Ballpoint Pen Set", Description = "Set of 10 premium ballpoint pens", Price = 8.99m, StockQuantity = 300, CategoryId = 3, Sku = "STAT-002", IsActive = true, CreatedAt = DateTime.UtcNow },
            };
        }

        public Task<List<Product>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_products.ToList());
            }
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            }
        }

        public Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            lock (_lock)
            {
                var products = _products.Where(p => p.CategoryId == categoryId).ToList();
                return Task.FromResult(products);
            }
        }

        public Task<Product> CreateAsync(Product product)
        {
            lock (_lock)
            {
                product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
                product.CreatedAt = DateTime.UtcNow;
                _products.Add(product);
                SaveToFile(_products);
                return Task.FromResult(product);
            }
        }

        public Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                    return Task.FromResult<Product?>(null);

                if (request.Name != null) product.Name = request.Name;
                if (request.Description != null) product.Description = request.Description;
                if (request.Price.HasValue) product.Price = request.Price.Value;
                if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
                if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId.Value;
                if (request.Sku != null) product.Sku = request.Sku;
                if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
                product.UpdatedAt = DateTime.UtcNow;

                SaveToFile(_products);
                return Task.FromResult<Product?>(product);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                    return Task.FromResult(false);

                _products.Remove(product);
                SaveToFile(_products);
                return Task.FromResult(true);
            }
        }

        public Task<List<Product>> SearchAsync(string query)
        {
            lock (_lock)
            {
                var results = _products
                    .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                (p.Description != null && p.Description.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                                p.Sku.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                return Task.FromResult(results);
            }
        }
    }
}
