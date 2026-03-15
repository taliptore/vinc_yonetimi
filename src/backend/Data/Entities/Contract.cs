namespace VincYonetim.Api.Data.Entities;

public class Contract
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int FirmId { get; set; }
    public int? JobId { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? DocumentPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Firm Firm { get; set; } = null!;
    public Job? Job { get; set; }
}
