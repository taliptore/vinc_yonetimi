using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;
using Xunit;

namespace VincYonetim.Tests;

public class TenantIsolationTests
{
    private static ApplicationDbContext CreateDb(ICurrentTenant? tenant, string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options, tenant);
    }

    [Fact]
    public async Task FirmsQuery_Respects_TenantId_When_CurrentTenant_Set()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant1 = new CurrentTenant { TenantId = 1, UserId = 1, Role = "Admin" };
        var tenant2 = new CurrentTenant { TenantId = 2, UserId = 2, Role = "Admin" };

        await using (var db = CreateDb(null, dbName))
        {
            db.Tenants.AddRange(
                new Tenant { Id = 1, Name = "T1", CreatedAt = DateTime.UtcNow },
                new Tenant { Id = 2, Name = "T2", CreatedAt = DateTime.UtcNow });
            db.Firms.AddRange(
                new Firm { Id = 1, TenantId = 1, Name = "F1-T1", CreatedAt = DateTime.UtcNow },
                new Firm { Id = 2, TenantId = 2, Name = "F2-T2", CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }

        await using (var db1 = CreateDb(tenant1, dbName))
        {
            var list = await db1.Firms.Where(f => f.TenantId == tenant1.TenantId).ToListAsync();
            Assert.Single(list);
            Assert.Equal(1, list[0].TenantId);
            Assert.Equal("F1-T1", list[0].Name);
        }

        await using (var db2 = CreateDb(tenant2, dbName))
        {
            var list = await db2.Firms.Where(f => f.TenantId == tenant2.TenantId).ToListAsync();
            Assert.Single(list);
            Assert.Equal(2, list[0].TenantId);
            Assert.Equal("F2-T2", list[0].Name);
        }
    }

    [Fact]
    public void CurrentTenant_IsAuthenticated_When_TenantId_And_UserId_Set()
    {
        var t = new CurrentTenant { TenantId = 1, UserId = 1, Role = "Admin" };
        Assert.True(t.IsAuthenticated);
        t.TenantId = null;
        Assert.False(t.IsAuthenticated);
    }
}
