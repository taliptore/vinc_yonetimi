using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public JobsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<Job>>> List([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? operatorId, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var q = _db.Jobs
            .Include(x => x.Firm)
            .Include(x => x.Site)
            .Include(x => x.Crane)
            .Include(x => x.Operator)
            .Where(x => x.TenantId == _tenant.TenantId);
        if (from.HasValue) q = q.Where(x => x.EndDate >= from.Value);
        if (to.HasValue) q = q.Where(x => x.StartDate <= to.Value);
        if (operatorId.HasValue) q = q.Where(x => x.OperatorId == operatorId.Value);
        var list = await q.OrderByDescending(x => x.StartDate).ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Job>> Get(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Jobs
            .Include(x => x.Firm).Include(x => x.Site).Include(x => x.Crane).Include(x => x.Operator)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Job>> Create([FromBody] Job model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        model.TenantId = _tenant.TenantId.Value;
        model.Id = 0;
        _db.Jobs.Add(model);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] Job model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.FirmId = model.FirmId;
        item.SiteId = model.SiteId;
        item.CraneId = model.CraneId;
        item.OperatorId = model.OperatorId;
        item.StartDate = model.StartDate;
        item.EndDate = model.EndDate;
        item.DailyRentPrice = model.DailyRentPrice;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = _tenant.UserId;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
