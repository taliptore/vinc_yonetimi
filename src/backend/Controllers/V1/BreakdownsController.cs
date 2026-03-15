using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Operatör")]
public class BreakdownsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public BreakdownsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] BreakdownRequest model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenant.UserId, ct);
        if (user == null) return Forbid();
        var craneExists = await _db.Cranes.AnyAsync(c => c.Id == model.CraneId && c.TenantId == _tenant.TenantId, ct);
        if (!craneExists) return BadRequest(new { message = "Vinç bulunamadı." });
        _db.BreakdownReports.Add(new BreakdownReport
        {
            TenantId = _tenant.TenantId.Value,
            CraneId = model.CraneId,
            OperatorId = user.OperatorId,
            Date = DateTime.UtcNow,
            Description = model.Description ?? "",
            Status = "Beklemede",
        });
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Arıza bildirimi alındı." });
    }

    public record BreakdownRequest(int CraneId, string? Description);
}
