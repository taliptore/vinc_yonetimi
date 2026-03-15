namespace VincYonetim.Api.DTOs.Auth;

public record ResetPasswordRequest(string Token, string NewPassword);
