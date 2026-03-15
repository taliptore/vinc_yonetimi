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
public class InvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public InvoicesController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<List<object>>> List([FromQuery] int? firmId, [FromQuery] string? status, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var q = _db.Invoices.Where(x => x.TenantId == _tenant.TenantId);
        if (firmId.HasValue) q = q.Where(x => x.FirmId == firmId.Value);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(x => x.Status == status);
        var list = await q.OrderByDescending(x => x.CreatedAt)
            .Select(x => new { x.Id, x.FirmId, x.HakedisId, x.InvoiceNo, x.Amount, x.IssueDate, x.DueDate, x.Status, x.PdfPath, x.CreatedAt })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Invoice>> Get(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> Create([FromBody] Invoice model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        model.TenantId = _tenant.TenantId.Value;
        model.Id = 0;
        model.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(model.Status)) model.Status = "Beklemede";
        _db.Invoices.Add(model);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] Invoice model, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        item.HakedisId = model.HakedisId;
        item.FirmId = model.FirmId;
        item.InvoiceNo = model.InvoiceNo;
        item.Amount = model.Amount;
        item.IssueDate = model.IssueDate;
        item.DueDate = model.DueDate;
        item.Status = model.Status ?? item.Status;
        item.PdfPath = model.PdfPath;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var item = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenant.TenantId, ct);
        if (item == null) return NotFound();
        _db.Invoices.Remove(item);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
