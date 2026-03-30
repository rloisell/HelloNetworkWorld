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
/// A network reference link displayed on the /docs panel.
/// Covers OpenShift SDN, cluster tier networking, DataClass/zones, NetworkPolicy patterns.
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
/// Display category for network reference links in the /docs panel.
/// Pivoted March 2026: narrowed from general BC Gov standards to network-specific content.
/// </summary>
public enum ReferenceLinkCategory
{
    OpenShiftNetworking,    // OpenShift SDN, OVN-Kubernetes, NetworkPolicy overview
    ClusterTiers,           // Silver / Gold / Emerald cluster networking differences
    DataClassAndZones,      // BC Gov DataClass labels, network zone model, Emerald AVI impact
    NetworkPolicyPatterns,  // Two-policy rule, DNS egress, CIDR allowances, common ports
    SdnGuidance             // Default-deny, troubleshooting, oc debug, Calico vs OVN
}
