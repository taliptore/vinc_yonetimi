using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/audit-logs")]
[Authorize(Roles = "Admin,Muhasebe")]
public class AuditLogsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public AuditLogsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<object>>> List(
        [FromQuery] string? entityType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int take = 100,
        CancellationToken ct = default)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var q = _db.AuditLogs.Where(x => x.TenantId == _tenant.TenantId);
        if (!string.IsNullOrWhiteSpace(entityType))
            q = q.Where(x => x.EntityType == entityType);
        if (from.HasValue) q = q.Where(x => x.CreatedAt >= from.Value);
        if (to.HasValue) q = q.Where(x => x.CreatedAt <= to.Value);
        var list = await q.OrderByDescending(x => x.CreatedAt)
            .Take(Math.Min(take, 500))
            .Select(x => new { x.Id, x.UserId, x.Action, x.EntityType, x.EntityId, x.OldValues, x.NewValues, x.CreatedAt })
            .ToListAsync(ct);
        return Ok(list);
    }
}
