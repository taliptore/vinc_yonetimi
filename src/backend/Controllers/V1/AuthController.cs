using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VincYonetim.Api.Data;
using VincYonetim.Api.Data.Entities;
using VincYonetim.Api.DTOs.Auth;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _db;

    public AuthController(IAuthService authService, ApplicationDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Firma adı, e-posta ve şifre zorunludur." });
        var emailNorm = request.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == emailNorm, ct))
            return BadRequest(new { message = "Bu e-posta adresi zaten kayıtlı." });
        var tenant = new Tenant { Name = request.CompanyName.Trim(), CreatedAt = DateTime.UtcNow };
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);
        var adminUser = new User
        {
            TenantId = tenant.Id,
            RoleId = 1,
            Email = emailNorm,
            DisplayName = request.AdminName?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(adminUser);
        await _db.SaveChangesAsync(ct);
        tenant.OwnerAdminId = adminUser.Id;
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Hesabınız oluşturuldu. Giriş yapabilirsiniz.", tenantId = tenant.Id });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result == null)
            return Unauthorized(new { message = "E-posta veya şifre hatalı." });
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(request.Email, cancellationToken);
        return Ok(new { message = "E-posta gönderildi. Şifre sıfırlama linkini kontrol edin." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var ok = await _authService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);
        if (!ok) return BadRequest(new { message = "Geçersiz veya süresi dolmuş token." });
        return Ok(new { message = "Şifre güncellendi." });
    }
}
