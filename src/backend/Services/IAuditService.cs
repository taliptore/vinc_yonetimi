namespace VincYonetim.Api.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default);
}
