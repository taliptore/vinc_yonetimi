using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.DTOs.Tenants;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class TenantsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public TenantsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    /// <summary>Yeni tenant (firma) + ilk admin kullanıcı. Sadece kendi tenant'ının OwnerAdmin'i oluşturabilir.</summary>
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateTenantRequest request, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var currentTenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == _tenant.TenantId.Value, ct);
        if (currentTenant?.OwnerAdminId != _tenant.UserId.Value)
            return Forbid();

        var newTenant = new Tenant { Name = request.Name.Trim(), CreatedAt = DateTime.UtcNow };
        _db.Tenants.Add(newTenant);
        await _db.SaveChangesAsync(ct);

        var adminUser = new User
        {
            TenantId = newTenant.Id,
            RoleId = 1,
            Email = request.AdminEmail.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(adminUser);
        await _db.SaveChangesAsync(ct);

        newTenant.OwnerAdminId = adminUser.Id;
        await _db.SaveChangesAsync(ct);

        return Ok(new { tenantId = newTenant.Id, adminUserId = adminUser.Id, message = "Tenant ve admin oluşturuldu." });
    }
}
