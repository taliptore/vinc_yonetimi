using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public DashboardController(ApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<ActionResult<object>> Get(CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue) return Unauthorized();
        var tenantId = _tenant.TenantId.Value;
        var role = _tenant.Role ?? "";

        if (role == "Admin")
        {
            var totalCranes = await _db.Cranes.CountAsync(x => x.TenantId == tenantId, ct);
            var totalFirms = await _db.Firms.CountAsync(x => x.TenantId == tenantId, ct);
            var activeJobs = await _db.Jobs.CountAsync(x => x.TenantId == tenantId && x.EndDate >= DateTime.Today, ct);
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var monthlyIncome = await _db.HakedisList
                .Where(h => h.Job != null && h.Job.TenantId == tenantId && h.CreatedAt >= monthStart)
                .SumAsync(h => h.NetAmount, ct);
            var monthlyFuel = await _db.FuelLogs
                .Where(f => f.TenantId == tenantId && f.Date >= monthStart)
                .SumAsync(f => f.Amount, ct);
            var monthlyMaintenance = await _db.MaintenanceRecords
                .Where(m => m.TenantId == tenantId && m.MaintenanceDate >= monthStart)
                .SumAsync(m => m.Cost ?? 0, ct);

            var monthlyBreakdown = new List<object>();
            for (var i = 5; i >= 0; i--)
            {
                var d = DateTime.Today.AddMonths(-i);
                var start = new DateTime(d.Year, d.Month, 1);
                var end = start.AddMonths(1);
                var total = await _db.HakedisList
                    .Where(h => h.Job != null && h.Job.TenantId == tenantId && h.CreatedAt >= start && h.CreatedAt < end)
                    .SumAsync(h => h.NetAmount, ct);
                monthlyBreakdown.Add(new { month = start.ToString("yyyy-MM"), label = start.ToString("MM/yyyy"), total });
            }

            return Ok(new
            {
                totalCranes,
                totalFirms,
                activeJobs,
                monthlyIncome,
                monthlyFuel,
                monthlyMaintenance,
                monthlyIncomeBreakdown = monthlyBreakdown
            });
        }

        if (role == "Muhasebe")
        {
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var hakedisSum = await _db.HakedisList
                .Where(h => h.Job != null && h.Job.TenantId == tenantId && h.CreatedAt >= monthStart)
                .SumAsync(h => h.NetAmount, ct);
            var paymentsSum = await _db.Payments.Where(p => p.Date >= monthStart).SumAsync(p => p.Amount, ct);
            var advancesSum = await _db.Advances.Where(a => a.TenantId == tenantId && a.Date >= monthStart).SumAsync(a => a.Amount, ct);
            var fuelSum = await _db.FuelLogs.Where(f => f.TenantId == tenantId && f.Date >= monthStart).SumAsync(f => f.Amount, ct);

            return Ok(new { hakedisSum, paymentsSum, advancesSum, fuelSum });
        }

        if (role == "Operatör")
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenant.UserId, ct);
            if (user?.OperatorId == null)
                return Ok(new { todayJob = (object?)null, site = (string?)null, totalHoursToday = 0m });

            var today = DateTime.Today;
            var todayJob = await _db.Jobs
                .Include(j => j.Site).Include(j => j.Crane)
                .FirstOrDefaultAsync(j => j.TenantId == tenantId && j.OperatorId == user.OperatorId && j.StartDate <= today && j.EndDate >= today, ct);
            var dayWork = await _db.OperatorDailyWorks
                .FirstOrDefaultAsync(d => d.OperatorId == user.OperatorId && d.WorkDate == today, ct);

            return Ok(new
            {
                todayJob = todayJob == null ? null : new { todayJob.Id, todayJob.Site?.Name, todayJob.Crane?.Plate },
                site = todayJob?.Site?.Name,
                totalHoursToday = dayWork?.TotalHours ?? 0
            });
        }

        if (role == "Firma")
        {
            var firmId = _db.Users.Where(u => u.Id == _tenant.UserId).Select(u => u.FirmId).FirstOrDefault();
            if (!firmId.HasValue) return Ok(new { rentedCranes = 0, activeJobs = 0, hakedisSummary = 0m });

            var activeJobs = await _db.Jobs.CountAsync(j => j.TenantId == tenantId && j.FirmId == firmId && j.EndDate >= DateTime.Today, ct);
            var rentedCranes = await _db.Jobs.Where(j => j.FirmId == firmId && j.EndDate >= DateTime.Today).Select(j => j.CraneId).Distinct().CountAsync(ct);
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var hakedisSummary = await _db.HakedisList
                .Where(h => h.Job != null && h.Job.FirmId == firmId && h.CreatedAt >= monthStart)
                .SumAsync(h => h.NetAmount, ct);

            return Ok(new { rentedCranes, activeJobs, hakedisSummary });
        }

        return Ok(new { });
    }
}
