namespace VincYonetim.Api.Data.Entities;

public class User
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public int? OperatorId { get; set; }
    public int? FirmId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
