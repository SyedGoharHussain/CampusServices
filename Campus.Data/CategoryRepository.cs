using Campus.Models;
using Microsoft.EntityFrameworkCore;

namespace Campus.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) { _db = db; }

    public Task<bool> ExistsAsync(int id) => _db.Categories.AnyAsync(c => c.Id == id);

    public Task<List<CategoryDto>> GetAllAsync() =>
        _db.Categories.AsNoTracking()
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
}
