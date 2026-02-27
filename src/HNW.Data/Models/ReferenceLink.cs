/*
 * ReferenceLink.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * EF Core entity for a documentation hub reference link.
 * Seeded on startup from the catalogue defined in kitty-specs/002-documentation-hub/spec.md.
 * AI-assisted: entity property scaffolding; reviewed and directed by Ryan Loiselle.
 */

namespace HNW.Data.Models;

/// <summary>
/// A documentation reference link displayed on the /docs page.
/// IsEnvironmentRelative links are built dynamically from cluster context env vars.
/// </summary>
public class ReferenceLink
{
    // ── IDENTITY ──────────────────────────────────────────────────────────────
    public Guid                 Id                    { get; set; } = Guid.NewGuid();
    public ReferenceLinkCategory Category             { get; set; }
    public string               Title                 { get; set; } = string.Empty;
    public string               Url                   { get; set; } = string.Empty;
    public string?              Description           { get; set; }

    // ── ENVIRONMENT AWARENESS ────────────────────────────────────────────────
    // When true, the Url is a template: {namespace} and {cluster} are substituted at runtime
    public bool  IsEnvironmentRelative { get; set; } = false;

    // ── DISPLAY ──────────────────────────────────────────────────────────────
    public int  SortOrder { get; set; } = 0;
    public bool IsActive  { get; set; } = true;

    // ── AUDIT ─────────────────────────────────────────────────────────────────
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

} // end ReferenceLink

// ── ENUMS ─────────────────────────────────────────────────────────────────────

/// <summary>
/// Display category for reference links in the documentation hub.
/// </summary>
public enum ReferenceLinkCategory
{
    Design,           // BC Gov Design System, BC Sans, UX guidelines
    Development,      // Developer portal, GitHub orgs, Rocket.Chat
    Security,         // Info security policy, STRA, CodeQL, Vault
    OpenShift,        // Emerald console, ArgoCD, Artifactory, Platform Registry
    GitOps,           // ArgoCD, Helm, GitHub Actions patterns
    AIGuidance,       // Copilot, agent skills, rl-project-template AI docs
    LocalEnvironment  // Dynamic links specific to this deployment (environment-relative)
}
