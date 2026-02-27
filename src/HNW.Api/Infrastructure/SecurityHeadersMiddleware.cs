/*
 * SecurityHeadersMiddleware.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Middleware that adds security headers to all HTTP responses.
 * Headers: X-Content-Type-Options, X-Frame-Options, Referrer-Policy, Permissions-Policy.
 * AI-assisted: header middleware scaffolding; reviewed and directed by Ryan Loiselle.
 */

namespace HNW.Api.Infrastructure;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    // adds security headers to every response before passing to next middleware
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
        await next(context);
    }

} // end SecurityHeadersMiddleware

// ── EXTENSION METHOD ─────────────────────────────────────────────────────────

public static class SecurityHeadersMiddlewareExtensions
{
    // registers SecurityHeadersMiddleware in the HTTP pipeline
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
} // end SecurityHeadersMiddlewareExtensions
