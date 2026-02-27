/*
 * NetworkTestStateChange.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Records a state transition (pass → fail or fail → pass) for a network test.
 * These events drive the state changes timeline in the reporting dashboard.
 * AI-assisted: entity property scaffolding; reviewed and directed by Ryan Loiselle.
 */

namespace HNW.Data.Models;

/// <summary>
/// A recorded pass↔fail state transition for a <see cref="NetworkTestDefinition"/>.
/// </summary>
public class NetworkTestStateChange
{
    // ── IDENTITY ──────────────────────────────────────────────────────────────
    public Guid              Id                       { get; set; } = Guid.NewGuid();
    public Guid              NetworkTestDefinitionId  { get; set; }
    public NetworkTestDefinition? NetworkTestDefinition { get; set; }

    // ── TRANSITION ────────────────────────────────────────────────────────────
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool           OldState  { get; set; }  // false = was failing, true = was passing
    public bool           NewState  { get; set; }  // false = now failing, true = now passing
    public string?        Note      { get; set; }  // optional context (e.g. "connectivity restored")

} // end NetworkTestStateChange
