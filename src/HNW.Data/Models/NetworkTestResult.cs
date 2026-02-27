/*
 * NetworkTestResult.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * EF Core entity storing the result of a single network test execution.
 * Results are never purged — full history is kept for trendline reporting.
 * AI-assisted: entity property scaffolding; reviewed and directed by Ryan Loiselle.
 */

namespace HNW.Data.Models;

/// <summary>
/// Result of a single execution of a <see cref="NetworkTestDefinition"/>.
/// Indexed on ExecutedAt and FK for efficient trendline queries.
/// </summary>
public class NetworkTestResult
{
    // ── IDENTITY ──────────────────────────────────────────────────────────────
    public Guid              Id                        { get; set; } = Guid.NewGuid();
    public Guid              NetworkTestDefinitionId   { get; set; }
    public NetworkTestDefinition? NetworkTestDefinition { get; set; }

    // ── RESULT ────────────────────────────────────────────────────────────────
    public DateTimeOffset ExecutedAt    { get; set; } = DateTimeOffset.UtcNow;
    public bool           IsSuccess     { get; set; }
    public int?           LatencyMs     { get; set; }  // round-trip time (null if probe failed before measuring)
    public int?           StatusCode    { get; set; }  // HTTP status code for HttpEndpoint/OidcProvider tests
    public string?        ErrorMessage  { get; set; }  // failure detail (null on success)

} // end NetworkTestResult
