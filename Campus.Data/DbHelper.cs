using Campus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Campus.Data;

public static class DbHelper
{
    // Reads the connection string. The value lives in user-secrets, never in source code.
    public static string GetConnectionString(IConfiguration config) =>
        config.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:Default. Set it with dotnet user-secrets.");

    // The only place that chooses the database provider (Supabase = PostgreSQL)
    public static void Configure(DbContextOptionsBuilder options, IConfiguration config) =>
        options.UseNpgsql(GetConnectionString(config));

    // Create tables + seed roles, categories and one staff user
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var r in new[] { "Student", "Staff" })
            if (!await roles.RoleExistsAsync(r)) await roles.CreateAsync(new IdentityRole(r));

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "IT Support" },
                new Category { Name = "Facilities" },
                new Category { Name = "Administration" });
            await db.SaveChangesAsync();
        }

        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var staffPassword = config["SeedStaffPassword"];
        if (staffPassword != null && await users.FindByEmailAsync("staff@campus.com") == null)
        {
            var staff = new IdentityUser { UserName = "staff@campus.com", Email = "staff@campus.com" };
            await users.CreateAsync(staff, staffPassword);
            await users.AddToRoleAsync(staff, "Staff");
        }
    }
}
