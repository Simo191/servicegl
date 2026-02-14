namespace MultiServices.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // ⚠️ X-XSS-Protection est obsolète sur Chrome, mais tu peux le laisser
        context.Response.Headers["X-XSS-Protection"] = "0";

        // HSTS uniquement en PROD (sinon ça peut gêner en dev)
        if (!_env.IsDevelopment())
        {
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(self)";

        // ✅ CSP adaptée
        var path = context.Request.Path;

        if (_env.IsDevelopment() && path.StartsWithSegments("/swagger"))
        {
            // Swagger a besoin de inline JS/CSS (sinon écran cassé)
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "connect-src 'self' https: wss: http://localhost:* ws://localhost:*;";
        }
        else
        {
            // PROD (strict)
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "img-src 'self' data:; " +
                "object-src 'none'; " +
                "base-uri 'self'; " +
                "frame-ancestors 'none';";
        }

        await _next(context);
    }
}
