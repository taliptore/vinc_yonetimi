using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VincYonetim.Api.Data;
using VincYonetim.Api.DTOs.Auth;

namespace VincYonetim.Api.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthService(ApplicationDbContext db, IConfiguration config, IEmailService emailService)
    {
        _db = db;
        _config = config;
        _emailService = emailService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = GenerateJwt(user.TenantId, user.Id, user.Email, user.Role.Name);
        var expires = DateTime.UtcNow.AddHours(2);

        return new LoginResponse(token, user.Email, user.Role.Name, user.TenantId, user.Id, expires);
    }

    private string GenerateJwt(int tenantId, int userId, string email, string role)
    {
        var key = _config["Jwt:Key"] ?? "VincYonetim-SuperSecretKey-Min32Chars!!";
        var issuer = _config["Jwt:Issuer"] ?? "VincYonetim.Api";
        var audience = _config["Jwt:Audience"] ?? "VincYonetim";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("tenantId", tenantId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null) return true; // Güvenlik: e-posta yoksa da başarılı dön

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "_").Replace("+", "-").TrimEnd('=');
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _db.SaveChangesAsync(cancellationToken);

        var baseUrl = _config["App:BaseUrl"] ?? "http://localhost:5042";
        var resetLink = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
        await _emailService.SendPasswordResetAsync(user.Email, resetLink, cancellationToken);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTime.UtcNow, cancellationToken);
        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
