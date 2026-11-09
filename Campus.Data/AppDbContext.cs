using Campus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Campus.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<ServiceRequest>().HasOne(r => r.CreatedBy).WithMany()
            .HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ServiceRequest>().HasOne(r => r.AssignedTo).WithMany()
            .HasForeignKey(r => r.AssignedToId).OnDelete(DeleteBehavior.Restrict);
    }
}
