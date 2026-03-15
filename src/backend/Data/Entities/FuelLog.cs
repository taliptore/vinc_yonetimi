namespace VincYonetim.Api.Data.Entities;

public class FuelLog
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CraneId { get; set; }
    public DateTime Date { get; set; }
    public decimal Liters { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Crane Crane { get; set; } = null!;
}
