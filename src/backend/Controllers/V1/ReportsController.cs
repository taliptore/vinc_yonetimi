using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Muhasebe")]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public ReportsController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet("crane-usage")]
    public async Task<ActionResult<object>> CraneUsage([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;
        var days = (toDate - fromDate).Days + 1;
        var works = await _db.OperatorDailyWorks
            .Where(w => w.Job != null && w.Job.TenantId == _tenant.TenantId && w.WorkDate >= fromDate && w.WorkDate <= toDate)
            .GroupBy(w => w.Job!.CraneId)
            .Select(g => new { craneId = g.Key, workDays = g.Select(x => x.WorkDate).Distinct().Count() })
            .ToListAsync(ct);
        var craneIds = works.Select(x => x.craneId).ToList();
        var cranes = await _db.Cranes.Where(c => c.TenantId == _tenant.TenantId && craneIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, ct);
        var list = works.Select(w => new
        {
            craneId = w.craneId,
            cranePlate = cranes.GetValueOrDefault(w.craneId)?.Plate,
            workDays = w.workDays,
            usagePercent = days > 0 ? Math.Round((double)w.workDays / days * 100, 1) : 0
        }).ToList();
        return Ok(new { fromDate, toDate, totalDays = days, items = list });
    }

    [HttpGet("income")]
    public async Task<ActionResult<object>> Income([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;
        var q = _db.HakedisList.Where(h => h.Job != null && h.Job.TenantId == _tenant.TenantId && h.CreatedAt >= fromDate && h.CreatedAt <= toDate);
        var total = await q.SumAsync(h => h.NetAmount, ct);
        var count = await q.CountAsync(ct);
        return Ok(new { fromDate, toDate, totalIncome = total, hakedisCount = count });
    }

    [HttpGet("fuel")]
    public async Task<ActionResult<object>> Fuel([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;
        var list = await _db.FuelLogs
            .Where(f => f.TenantId == _tenant.TenantId && f.Date >= fromDate && f.Date <= toDate)
            .GroupBy(f => f.CraneId)
            .Select(g => new { craneId = g.Key, totalLiters = g.Sum(x => x.Liters), totalAmount = g.Sum(x => x.Amount) })
            .ToListAsync(ct);
        var totalAmount = await _db.FuelLogs
            .Where(f => f.TenantId == _tenant.TenantId && f.Date >= fromDate && f.Date <= toDate)
            .SumAsync(f => f.Amount, ct);
        return Ok(new { fromDate, toDate, items = list, totalAmount });
    }

    [HttpGet("operator-performance")]
    public async Task<ActionResult<object>> OperatorPerformance([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;
        var list = await _db.OperatorDailyWorks
            .Where(w => w.Job != null && w.Job.TenantId == _tenant.TenantId && w.WorkDate >= fromDate && w.WorkDate <= toDate)
            .GroupBy(w => w.OperatorId)
            .Select(g => new
            {
                operatorId = g.Key,
                totalDays = g.Select(x => x.WorkDate).Distinct().Count(),
                totalHours = g.Sum(x => x.TotalHours),
                overtimeHours = g.Sum(x => x.OvertimeHours)
            })
            .ToListAsync(ct);
        var opIds = list.Select(x => x.operatorId).ToList();
        var operators = await _db.Operators.Where(o => o.TenantId == _tenant.TenantId && opIds.Contains(o.Id)).ToDictionaryAsync(o => o.Id, ct);
        var result = list.Select(x => new
        {
            operatorId = x.operatorId,
            operatorName = operators.GetValueOrDefault(x.operatorId)?.FullName,
            totalDays = x.totalDays,
            totalHours = x.totalHours,
            overtimeHours = x.overtimeHours
        }).ToList();
        return Ok(new { fromDate, toDate, items = result });
    }

    [HttpGet("firm-jobs")]
    public async Task<ActionResult<object>> FirmJobs([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;
        var list = await _db.Jobs
            .Where(j => j.TenantId == _tenant.TenantId && j.StartDate <= toDate && j.EndDate >= fromDate)
            .GroupBy(j => j.FirmId)
            .Select(g => new { firmId = g.Key, jobCount = g.Count() })
            .ToListAsync(ct);
        var firmIds = list.Select(x => x.firmId).ToList();
        var firms = await _db.Firms.Where(f => f.TenantId == _tenant.TenantId && firmIds.Contains(f.Id)).ToDictionaryAsync(f => f.Id, ct);
        var result = list.Select(x => new { firmId = x.firmId, firmName = firms.GetValueOrDefault(x.firmId)?.Name, jobCount = x.jobCount }).ToList();
        return Ok(new { fromDate, toDate, items = result });
    }
}
