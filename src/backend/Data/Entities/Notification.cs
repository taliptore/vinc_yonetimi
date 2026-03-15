namespace VincYonetim.Api.Data.Entities;

public class Notification
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; } = "Info"; // Info, Alert, Job, Breakdown, Payment
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
}
