/*
 * Program.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * ASP.NET Core 10 composition root for HelloNetworkWorld API.
 * Registers: CORS, EF Core (Pomelo MariaDB), Quartz.NET scheduler,
 * health checks, Swagger, global exception handler, rate limiter,
 * domain services, and HTTP middleware pipeline.
 * AI-assisted: composition root scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Api.Infrastructure;
using HNW.Api.Services;
using HNW.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins("http://localhost:5175", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());

    options.AddPolicy("ProdCors", policy =>
        policy.WithOrigins(
            builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ── EF Core / MariaDB ────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── Health Checks ────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddMySql(connectionString, name: "mariadb", tags: new[] { "ready" });

// ── Quartz.NET Scheduler ─────────────────────────────────────────────────────
builder.Services.AddQuartz(q =>
{
    // Job store: RAMJobStore for Phase 1 (simple, no HA)
    // Switch to Quartz.Plugins.TimeZoneConverter + RAMJobStore for now
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ── Controllers / Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HelloNetworkWorld API", Version = "v1" });
});

// ── Global Exception Handling / ProblemDetails ───────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── Domain Services ──────────────────────────────────────────────────────────
builder.Services.AddScoped<INetworkTestService, NetworkTestService>();
builder.Services.AddScoped<IReferenceLinkService, ReferenceLinkService>();
// Scheduler and probe services registered in feature 004

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
var app = builder.Build();

// Auto-apply EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}
else
{
    app.UseCors("ProdCors");
}

app.UseSecurityHeaders();
app.UseAuthorization();
app.MapControllers();

// ── Health Check Endpoints ────────────────────────────────────────────────────
// /health/live — always 200 (pod is running)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = async (ctx, _) =>
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync("{\"status\":\"Healthy\"}");
    }
});

// /health/ready — 200 when DB is reachable, 503 otherwise
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthCheckResponse
});

// /health/network — stub (extended by feature 004)
app.MapGet("/health/network", () => Results.Ok(new
{
    overallStatus = "Healthy",
    tests = Array.Empty<object>()
}));

// /api — project info
app.MapGet("/api", () => Results.Ok(new
{
    project = "HelloNetworkWorld",
    version = "0.1.0",
    description = "BC Gov network health check and standards reference hub"
}));

app.Run();

// ── HELPERS ──────────────────────────────────────────────────────────────────
// writes a consistent JSON body for health check responses
static async Task WriteHealthCheckResponse(HttpContext ctx, HealthReport report)
{
    ctx.Response.ContentType = "application/json";
    var result = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description
        })
    };
    await ctx.Response.WriteAsync(JsonSerializer.Serialize(result));
}
