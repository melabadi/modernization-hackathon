using LegacyInventoryApi.Models;
using Newtonsoft.Json;

namespace LegacyInventoryApi.Repositories
{
    public class JsonCategoryRepository : ICategoryRepository
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private List<Category> _categories;

        public JsonCategoryRepository(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "categories.json");
            _categories = LoadFromFile();
        }

        private List<Category> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                var seedData = GetSeedData();
                SaveToFile(seedData);
                return seedData;
            }

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Category>>(json) ?? new List<Category>();
        }

        private void SaveToFile(List<Category> categories)
        {
            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        private List<Category> GetSeedData()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories", CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Furniture", Description = "Office and home furniture", CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Stationery", Description = "Writing and office supplies", CreatedAt = DateTime.UtcNow },
            };
        }

        public Task<List<Category>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_categories.ToList());
            }
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(category);
            }
        }

        public Task<Category> CreateAsync(Category category)
        {
            lock (_lock)
            {
                category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1;
                category.CreatedAt = DateTime.UtcNow;
                _categories.Add(category);
                SaveToFile(_categories);
                return Task.FromResult(category);
            }
        }

        public Task<Category?> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            lock (_lock)
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                    return Task.FromResult<Category?>(null);

                if (request.Name != null) category.Name = request.Name;
                if (request.Description != null) category.Description = request.Description;

                SaveToFile(_categories);
                return Task.FromResult<Category?>(category);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            lock (_lock)
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                    return Task.FromResult(false);

                _categories.Remove(category);
                SaveToFile(_categories);
                return Task.FromResult(true);
            }
        }
    }
}
