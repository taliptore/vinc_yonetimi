namespace VincYonetim.Api.Data.Entities;

public class Site : ISoftDelete
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int FirmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public double? GpsLat { get; set; }
    public double? GpsLng { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Firm Firm { get; set; } = null!;
}
