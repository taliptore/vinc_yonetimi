using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class OperatorsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public OperatorsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<Operator>>> List(CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var list = await _db.Operators
            .Include(x => x.Crane)
            .Where(x => x.TenantId == _tenant.TenantId)
            .OrderBy(x => x.FullName)
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Operator>> Get(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Operators
            .Include(x => x.Crane)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Operator>> Create([FromBody] Operator model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        model.TenantId = _tenant.TenantId.Value;
        model.Id = 0;
        _db.Operators.Add(model);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] Operator model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Operators.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.FullName = model.FullName;
        item.Phone = model.Phone;
        item.Email = model.Email;
        item.LicenseType = model.LicenseType;
        item.CraneId = model.CraneId;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Operators.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = _tenant.UserId;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
