namespace VincYonetim.Api.Data.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? HakedisId { get; set; }
    public int FirmId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "Beklemede";
    public string? PdfPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Hakedis? Hakedis { get; set; }
    public Firm Firm { get; set; } = null!;
}
