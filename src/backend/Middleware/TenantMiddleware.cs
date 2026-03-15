using System.Security.Claims;
using VincYonetim.Api.Services;

namespace VincYonetim.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant)
    {
        if (currentTenant is CurrentTenant ct && context.User.Identity?.IsAuthenticated == true)
        {
            var tenantId = context.User.FindFirstValue("tenantId");
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            if (int.TryParse(tenantId, out var tid)) ct.TenantId = tid;
            if (int.TryParse(userId, out var uid)) ct.UserId = uid;
            ct.Role = role;
        }

        await _next(context);
    }
}
