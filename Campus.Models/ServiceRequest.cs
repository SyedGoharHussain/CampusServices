using Microsoft.AspNetCore.Identity;

namespace Campus.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public RequestStatus Status { get; set; } = RequestStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string CreatedById { get; set; } = "";
    public IdentityUser CreatedBy { get; set; } = null!;

    public string? AssignedToId { get; set; }
    public IdentityUser? AssignedTo { get; set; }
}
