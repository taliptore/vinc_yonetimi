using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Muhasebe")]
public class SitesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public SitesController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<Site>>> List([FromQuery] int? firmId, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var q = _db.Sites.Include(x => x.Firm).Where(x => x.TenantId == _tenant.TenantId);
        if (firmId.HasValue) q = q.Where(x => x.FirmId == firmId.Value);
        var list = await q.OrderBy(x => x.Name).ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Site>> Get(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Sites.Include(x => x.Firm).FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Site>> Create([FromBody] Site model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        model.TenantId = _tenant.TenantId.Value;
        model.Id = 0;
        _db.Sites.Add(model);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] Site model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Sites.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.FirmId = model.FirmId;
        item.Name = model.Name;
        item.Address = model.Address;
        item.City = model.City;
        item.GpsLat = model.GpsLat;
        item.GpsLng = model.GpsLng;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Sites.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = _tenant.UserId;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
