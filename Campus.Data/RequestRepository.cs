using Campus.Models;
using Microsoft.EntityFrameworkCore;

namespace Campus.Data;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _db;
    public RequestRepository(AppDbContext db) { _db = db; }

    // One JOIN query, only the columns we need (no N+1)
    private static IQueryable<RequestDto> Project(IQueryable<ServiceRequest> q) =>
        q.Select(r => new RequestDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Status = r.Status,
            Category = r.Category.Name,
            AssignedTo = r.AssignedTo != null ? r.AssignedTo.UserName : null,
            CreatedAt = r.CreatedAt
        });

    public async Task<List<RequestDto>> GetListAsync(string? ownerId)
    {
        var q = _db.ServiceRequests.AsNoTracking();               // read-only
        if (ownerId != null) q = q.Where(r => r.CreatedById == ownerId);
        return await Project(q.OrderByDescending(r => r.CreatedAt)).ToListAsync();
    }

    public async Task<RequestDto?> GetByIdAsync(int id, string? ownerId)
    {
        var q = _db.ServiceRequests.AsNoTracking().Where(r => r.Id == id);
        if (ownerId != null) q = q.Where(r => r.CreatedById == ownerId);
        return await Project(q).FirstOrDefaultAsync();
    }

    public async Task<ServiceRequest?> FindAsync(int id) =>
        await _db.ServiceRequests.FindAsync(id);                  // tracked on purpose

    public async Task AddAsync(ServiceRequest request)
    {
        _db.ServiceRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task SaveAsync() => await _db.SaveChangesAsync();  // EF writes only changed columns
}
