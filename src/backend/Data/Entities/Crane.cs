namespace VincYonetim.Api.Data.Entities;

public class Crane : ISoftDelete
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal? Tonnage { get; set; }
    public int? Year { get; set; }
    public string Status { get; set; } = "Aktif"; // Aktif, Pasif, Bakımda
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
}
