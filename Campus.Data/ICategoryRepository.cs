using Campus.Models;

namespace Campus.Data;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(int id);
    Task<List<CategoryDto>> GetAllAsync();
}
