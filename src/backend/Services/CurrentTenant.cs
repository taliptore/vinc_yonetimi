namespace VincYonetim.Api.Services;

public class CurrentTenant : ICurrentTenant
{
    public int? TenantId { get; set; }
    public int? UserId { get; set; }
    public string? Role { get; set; }
    public bool IsAuthenticated => TenantId.HasValue && UserId.HasValue;
}
