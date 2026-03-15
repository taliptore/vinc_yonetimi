namespace VincYonetim.Api.Data.Entities;

public class BreakdownReport
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CraneId { get; set; }
    public int? OperatorId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Beklemede"; // Beklemede, İnceleniyor, Tamamlandı
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Crane Crane { get; set; } = null!;
    public Operator? Operator { get; set; }
}
