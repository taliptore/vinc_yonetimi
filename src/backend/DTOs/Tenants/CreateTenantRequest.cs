namespace VincYonetim.Api.DTOs.Tenants;

public record CreateTenantRequest(string Name, string AdminEmail, string AdminPassword);
