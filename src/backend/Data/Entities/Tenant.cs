namespace VincYonetim.Api.Data.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? OwnerAdminId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? OwnerAdmin { get; set; }
}
