/*
 * GlobalExceptionHandler.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Global exception handler that maps domain exceptions to RFC 7807 ProblemDetails responses.
 * Catches NotFoundException (404), ForbiddenException (403), BadRequestException (400),
 * UnauthorizedException (401). All other exceptions return 500.
 * AI-assisted: exception handler scaffolding; reviewed and directed by Ryan Loiselle.
 */

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HNW.Api.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // handles the exception and writes a ProblemDetails response; returns true to stop propagation
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException   => (StatusCodes.Status404NotFound,       "Not Found"),
            ForbiddenException  => (StatusCodes.Status403Forbidden,      "Forbidden"),
            BadRequestException => (StatusCodes.Status400BadRequest,     "Bad Request"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _                   => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        if (statusCode == 500)
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title  = title,
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

} // end GlobalExceptionHandler

// ── DOMAIN EXCEPTIONS ────────────────────────────────────────────────────────

public class NotFoundException(string message) : Exception(message);
public class ForbiddenException(string message) : Exception(message);
public class BadRequestException(string message) : Exception(message);
public class UnauthorizedException(string message) : Exception(message);
