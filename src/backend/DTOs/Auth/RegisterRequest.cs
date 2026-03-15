namespace VincYonetim.Api.DTOs.Auth;

public record RegisterRequest(
    string CompanyName,
    string AdminName,
    string Email,
    string? Phone,
    string Password
);
