using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.DTOs.Jobs;
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

    /// <summary>İş başlat - QR + GPS doğrulama (200m). Operatör için.</summary>
    [HttpPost("{id:int}/start")]
    [Authorize(Roles = "Operatör")]
    public async Task<ActionResult> Start(int id, [FromBody] JobStartEndRequest request, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenant.UserId && u.OperatorId != null, ct);
        if (user == null) return Forbid();
        var job = await _db.Jobs.Include(j => j.Site).FirstOrDefaultAsync(j => j.Id == id && j.TenantId == _tenant.TenantId && j.OperatorId == user.OperatorId, ct);
        if (job == null) return NotFound();
        if (job.Site?.GpsLat != null && job.Site.GpsLng != null)
        {
            var dist = HaversineDistance(request.Latitude, request.Longitude, job.Site.GpsLat.Value, job.Site.GpsLng.Value);
            if (dist > 0.2) return BadRequest(new { message = "Şantiye konumuna 200m'den yakın olmalısınız.", distanceMetres = (int)(dist * 1000) });
        }
        var today = DateTime.UtcNow.Date;
        if (await _db.OperatorDailyWorks.AnyAsync(w => w.JobId == id && w.OperatorId == user.OperatorId && w.WorkDate == today, ct))
            return BadRequest(new { message = "Bu iş için bugün zaten başlangıç yapıldı." });
        var now = DateTime.UtcNow.TimeOfDay;
        _db.OperatorDailyWorks.Add(new OperatorDailyWork
        {
            JobId = id,
            OperatorId = user.OperatorId.Value,
            WorkDate = today,
            StartTime = now,
            EndTime = TimeSpan.Zero,
            TotalHours = 0,
            OvertimeHours = 0,
        });
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "İş başlatıldı." });
    }

    /// <summary>İş bitir - GPS doğrulama, yevmiye süresi hesaplanır.</summary>
    [HttpPost("{id:int}/end")]
    [Authorize(Roles = "Operatör")]
    public async Task<ActionResult> End(int id, [FromBody] JobStartEndRequest request, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue || !_tenant.UserId.HasValue) return Unauthorized();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenant.UserId && u.OperatorId != null, ct);
        if (user == null) return Forbid();
        var job = await _db.Jobs.Include(j => j.Site).FirstOrDefaultAsync(j => j.Id == id && j.TenantId == _tenant.TenantId && j.OperatorId == user.OperatorId, ct);
        if (job == null) return NotFound();
        if (job.Site?.GpsLat != null && job.Site.GpsLng != null)
        {
            var dist = HaversineDistance(request.Latitude, request.Longitude, job.Site.GpsLat.Value, job.Site.GpsLng.Value);
            if (dist > 0.2) return BadRequest(new { message = "Şantiye konumuna 200m'den yakın olmalısınız." });
        }
        var today = DateTime.UtcNow.Date;
        var work = await _db.OperatorDailyWorks.FirstOrDefaultAsync(w => w.JobId == id && w.OperatorId == user.OperatorId && w.WorkDate == today && w.EndTime == TimeSpan.Zero, ct);
        if (work == null) return BadRequest(new { message = "Bugün bu iş için başlangıç kaydı bulunamadı." });
        var endTime = DateTime.UtcNow.TimeOfDay;
        work.EndTime = endTime;
        var totalHours = (endTime - work.StartTime).TotalHours;
        if (totalHours < 0) totalHours += 24;
        work.TotalHours = (decimal)Math.Round(totalHours, 2);
        work.OvertimeHours = (decimal)Math.Max(0, Math.Round(totalHours - 8, 2));
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "İş bitirildi.", totalHours = work.TotalHours, overtimeHours = work.OvertimeHours });
    }

    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // km
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
