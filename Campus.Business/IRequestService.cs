using Campus.Models;

namespace Campus.Business;

public interface IRequestService
{
    Task<List<RequestDto>> GetListAsync(string userId, bool isStaff);
    Task<RequestDto?> GetByIdAsync(int id, string userId, bool isStaff);
    Task<RequestDto?> CreateAsync(CreateRequestDto dto, string userId);
    Task<bool> UpdateStatusAsync(int id, RequestStatus newStatus);
    Task<bool> AssignAsync(int id, string staffId);
    Task<List<CategoryDto>> GetCategoriesAsync();
}
