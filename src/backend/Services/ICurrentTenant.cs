namespace VincYonetim.Api.Services;

public interface ICurrentTenant
{
    int? TenantId { get; }
    int? UserId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
