using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public NotificationsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<object>>> List([FromQuery] bool unreadOnly, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var q = _db.Notifications
            .Where(x => x.TenantId == _tenant.TenantId && x.UserId == _tenant.UserId);
        if (unreadOnly) q = q.Where(x => !x.IsRead);
        var list = await q.OrderByDescending(x => x.CreatedAt).Take(50)
            .Select(x => new { x.Id, x.Title, x.Body, x.IsRead, x.Type, x.CreatedAt })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<ActionResult> MarkRead(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var item = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId && x.UserId == _tenant.UserId, ct);
        if (item == null) return NotFound();
        item.IsRead = true;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
