namespace VincYonetim.Api.Data.Entities;

public class Operator : ISoftDelete
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LicenseType { get; set; }
    public int? CraneId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Crane? Crane { get; set; }
}
