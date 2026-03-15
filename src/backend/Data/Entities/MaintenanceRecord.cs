namespace VincYonetim.Api.Data.Entities;

public class MaintenanceRecord
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CraneId { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string? Type { get; set; }
    public string? ServiceName { get; set; }
    public decimal? Cost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Crane Crane { get; set; } = null!;
}
