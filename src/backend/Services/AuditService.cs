using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;

namespace VincYonetim.Api.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenant _currentTenant;

    public AuditService(ApplicationDbContext db, ICurrentTenant currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task LogAsync(string action, string entityType, string? entityId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default)
    {
        if (!_currentTenant.TenantId.HasValue || !_currentTenant.UserId.HasValue)
            return;

        var oldJson = oldValues != null ? JsonSerializer.Serialize(oldValues) : null;
        var newJson = newValues != null ? JsonSerializer.Serialize(newValues) : null;

        _db.AuditLogs.Add(new AuditLog
        {
            TenantId = _currentTenant.TenantId.Value,
            UserId = _currentTenant.UserId.Value,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldJson,
            NewValues = newJson
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}
