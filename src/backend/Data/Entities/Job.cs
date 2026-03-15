namespace VincYonetim.Api.Data.Entities;

public class Job : ISoftDelete
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int FirmId { get; set; }
    public int SiteId { get; set; }
    public int CraneId { get; set; }
    public int? OperatorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DailyRentPrice { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Firm Firm { get; set; } = null!;
    public Site Site { get; set; } = null!;
    public Crane Crane { get; set; } = null!;
    public Operator? Operator { get; set; }
}
