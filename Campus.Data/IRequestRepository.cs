using Campus.Models;

namespace Campus.Data;

public interface IRequestRepository
{
    Task<List<RequestDto>> GetListAsync(string? ownerId);       // ownerId null = all requests
    Task<RequestDto?> GetByIdAsync(int id, string? ownerId);    // ownerId null = any owner
    Task<ServiceRequest?> FindAsync(int id);                    // tracked entity, for updating
    Task AddAsync(ServiceRequest request);
    Task SaveAsync();
}
