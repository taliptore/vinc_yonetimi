using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data.Entities;

namespace VincYonetim.Api.Data;

public static class DbSeed
{
    public static async Task EnsureSeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync(cancellationToken);

        if (await db.Tenants.AnyAsync(cancellationToken))
            return;

        var tenant = new Tenant
        {
            Name = "Varsayılan Firma",
            CreatedAt = DateTime.UtcNow
        };
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync(cancellationToken);

        var adminUser = new User
        {
            TenantId = tenant.Id,
            RoleId = 1,
            Email = "admin@vinc.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(adminUser);
        await db.SaveChangesAsync(cancellationToken);

        tenant.OwnerAdminId = adminUser.Id;
        await db.SaveChangesAsync(cancellationToken);
    }
}
