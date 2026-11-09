using Campus.Data;
using Campus.Models;

namespace Campus.Business;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requests;
    private readonly ICategoryRepository _categories;

    public RequestService(IRequestRepository requests, ICategoryRepository categories)
    {
        _requests = requests;
        _categories = categories;
    }

    // Business rule: which status can follow which
    private static readonly Dictionary<RequestStatus, RequestStatus[]> Allowed = new()
    {
        [RequestStatus.Open] = new[] { RequestStatus.Assigned },
        [RequestStatus.Assigned] = new[] { RequestStatus.InProgress },
        [RequestStatus.InProgress] = new[] { RequestStatus.Resolved },
        [RequestStatus.Resolved] = new[] { RequestStatus.Closed, RequestStatus.InProgress },
        [RequestStatus.Closed] = new RequestStatus[0]
    };

    // Check: a student only sees own requests, staff see all
    public Task<List<RequestDto>> GetListAsync(string userId, bool isStaff) =>
        _requests.GetListAsync(isStaff ? null : userId);

    public Task<RequestDto?> GetByIdAsync(int id, string userId, bool isStaff) =>
        _requests.GetByIdAsync(id, isStaff ? null : userId);

    public async Task<RequestDto?> CreateAsync(CreateRequestDto dto, string userId)
    {
        // Check: category must exist
        if (!await _categories.ExistsAsync(dto.CategoryId)) return null;

        var entity = new ServiceRequest
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            CreatedById = userId
        };
        await _requests.AddAsync(entity);
        return await _requests.GetByIdAsync(entity.Id, userId);
    }

    public async Task<bool> UpdateStatusAsync(int id, RequestStatus newStatus)
    {
        var r = await _requests.FindAsync(id);
        if (r == null) return false;

        // Check: status transition must be allowed
        if (!Allowed[r.Status].Contains(newStatus))
            throw new InvalidOperationException($"Cannot move from {r.Status} to {newStatus}");

        r.Status = newStatus;
        await _requests.SaveAsync();
        return true;
    }

    public async Task<bool> AssignAsync(int id, string staffId)
    {
        var r = await _requests.FindAsync(id);
        if (r == null) return false;

        r.AssignedToId = staffId;
        if (r.Status == RequestStatus.Open) r.Status = RequestStatus.Assigned;
        await _requests.SaveAsync();
        return true;
    }

    public Task<List<CategoryDto>> GetCategoriesAsync() => _categories.GetAllAsync();
}
