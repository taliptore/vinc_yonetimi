namespace VincYonetim.Api.Data.Entities;

public class Advance
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? OperatorId { get; set; }
    public int? JobId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Operator? Operator { get; set; }
    public Job? Job { get; set; }
}
