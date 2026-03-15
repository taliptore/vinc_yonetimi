using System.Collections.Concurrent;

namespace VincYonetim.Api.Middleware;

/// <summary>Login endpoint'ine istek sınırı (IP başına 10 deneme / 15 dakika).</summary>
public class LoginRateLimitMiddleware
{
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _attempts = new();
    private const int MaxAttempts = 10;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(15);
    private readonly RequestDelegate _next;
    private readonly ILogger<LoginRateLimitMiddleware> _logger;

    public LoginRateLimitMiddleware(RequestDelegate next, ILogger<LoginRateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/v1/auth/login", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = ip;
        var queue = _attempts.AddOrUpdate(key, _ => new Queue<DateTime>(), (_, q) => q);
        bool rateLimited;
        lock (queue)
        {
            var now = DateTime.UtcNow;
            while (queue.Count > 0 && now - queue.Peek() > Window)
                queue.Dequeue();
            rateLimited = queue.Count >= MaxAttempts;
            if (!rateLimited)
                queue.Enqueue(now);
        }
        if (rateLimited)
        {
            _logger.LogWarning("Login rate limit exceeded for {Ip}", ip);
            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Çok fazla giriş denemesi. 15 dakika sonra tekrar deneyin.\"}");
            return;
        }
        await _next(context);
    }
}
