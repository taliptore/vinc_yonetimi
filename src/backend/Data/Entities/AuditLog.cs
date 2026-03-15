namespace VincYonetim.Api.Data.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Created, Updated, Deleted
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
