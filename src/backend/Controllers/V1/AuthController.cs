using Microsoft.AspNetCore.Mvc;
using VincYonetim.Api.DTOs.Auth;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
