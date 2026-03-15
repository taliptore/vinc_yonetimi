namespace VincYonetim.Api.DTOs.Auth;

public record LoginResponse(string Token, string Email, string Role, int TenantId, int UserId, DateTime ExpiresAt);
