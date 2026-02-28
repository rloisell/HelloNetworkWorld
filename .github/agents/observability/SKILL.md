---
name: observability
description: Structured logging, metrics, and tracing for BC Gov .NET / React / OpenShift projects — Serilog JSON configuration, log level standards, PII-free logging rules, Prometheus pod annotations, OpenTelemetry .NET SDK setup, health check endpoints, and alert baseline. Use when configuring logging, adding metrics instrumentation, setting up health checks, or diagnosing production issues.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: .NET 10 / ASP.NET Core. Serilog 3+. OpenTelemetry .NET SDK 1.x. Emerald OpenShift — Prometheus pull model.
---

# Observability Agent

Standardises logging, metrics, tracing, and health checks across BC Gov projects.

---

## Logging — Serilog

### Package install
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Formatting.Compact
dotnet add package Serilog.Sinks.Console
```

### Program.cs bootstrap
```csharp
// LOGGING — structured JSON to stdout (picked up by OpenShift log aggregation)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();
```

### appsettings.json log levels
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

> Log level `Warning` for EF Core SQL commands — never log SQL at `Information` in production
> (may expose query parameters containing PII).

### Log level standards

| Level | Use |
|-------|-----|
| `Verbose` | Development only — detailed flow tracing |
| `Debug` | Diagnostic context helpful in test environments |
| `Information` | Normal application events (startup, user actions, state transitions) |
| `Warning` | Recoverable abnormal conditions (retry, missing optional config) |
| `Error` | Exceptions / failures that require attention |
| `Fatal` | Unrecoverable startup / crash events |

### PII-free logging rules

**NEVER** log:
- User names, email addresses
- SIN, DL number, student number, or other personal identifiers
- Connection strings, tokens, passwords, or Vault values
- Full HTTP request/response bodies (may contain form data)
- Query string parameters that may carry tokens (`?code=`, `?token=`)

**Safe to log:**
- User ID (GUID / sub claim) — opaque identifier only
- HTTP method + path (no query string)
- HTTP status code
- Duration (ms)
- Correlation ID / trace ID

### Structured logging pattern
```csharp
// Use structured properties, not string interpolation
_logger.LogInformation("Created work item {WorkItemId} for project {ProjectId}",
    item.Id, item.ProjectId);

// NOT:
_logger.LogInformation($"Created work item {item.Id}");
```

---

## Health Checks

### Package
```bash
dotnet add package AspNetCore.HealthChecks.MySql
```

### Registration in Program.cs
```csharp
// HEALTH CHECKS — /api/health (liveness) and /api/health/details (readiness)
builder.Services.AddHealthChecks()
    .AddMySql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "database",
        tags: ["ready"]);

// ...
app.MapHealthChecks("/api/health");
app.MapHealthChecks("/api/health/details", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate      = _ => true,
});
```

### Containerfile health check
```dockerfile
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -qO- http://localhost:8080/api/health || exit 1
```

---

## Metrics — Prometheus

Emerald uses a Prometheus pull model. Annotate pods so Prometheus discovers the metrics endpoint.

### Pod annotations (Helm values.yaml)
```yaml
podAnnotations:
  prometheus.io/scrape: "true"
  prometheus.io/path:   "/metrics"
  prometheus.io/port:   "8080"
```

### .NET metrics endpoint (ASP.NET Core)
```bash
dotnet add package prometheus-net.AspNetCore
```
```csharp
// METRICS — expose /metrics for Prometheus scrape
app.UseHttpMetrics();  // request duration, count, in-flight
app.MapMetrics();      // GET /metrics
```

### Custom metrics example
```csharp
// create once at class level (static) — Prometheus counters are global
private static readonly Counter _itemsCreated =
    Metrics.CreateCounter("app_items_created_total", "Number of work items created.",
        labelNames: ["project_id"]);

// in service method — increment counter when an item is created
_itemsCreated.WithLabels(projectId.ToString()).Inc();
```

---

## Tracing — OpenTelemetry

### Packages
```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Exporter.Console   # dev only
```

### Registration
```csharp
// TRACING — OpenTelemetry with OTLP export (or console in dev)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: "my-app-api"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();

        if (builder.Environment.IsDevelopment())
            tracing.AddConsoleExporter();
        else
            tracing.AddOtlpExporter(opt =>
                opt.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!));
    });
```

---

## Alert Baseline (document in securityNextSteps.md)

| Signal | Threshold | Action |
|--------|-----------|--------|
| HTTP 5xx rate | > 5% of requests over 5 min | Alert on-call |
| DB health check failing | > 2 consecutive failures | Alert on-call |
| Pod restart count | > 3 in 10 min | Alert on-call |
| High memory / CPU | > 90% of request limit for 5 min | Alert + scale |
| Auth failures (401/403) | Spike > 10× baseline | Security alert |

---

## OBSERVABILITY_KNOWLEDGE

```yaml
confirmed_facts:
  - "CompactJsonFormatter writes single-line JSON to stdout — required for OpenShift log aggregation"
  - "Prometheus on Emerald uses pod annotations to discover /metrics endpoints"
  - "EF Core SQL logging at Information level may expose query parameters — use Warning"
  - "OpenTelemetry AddEntityFrameworkCoreInstrumentation requires EFCore instrumentation package"
  - "Health check at /api/health is used by OpenShift liveness probe; /api/health/details for readiness"
common_pitfalls:
  - "Never log PII — user emails, names, or identifying numbers in structured properties"
  - "Metrics.CreateCounter must be static — creating per-request instances causes memory leaks"
  - "prometheus-net endpoint conflicts with ASP.NET Core minimal API route if MapMetrics() called after MapControllers()"
```
