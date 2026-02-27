/*
 * NetworkTestDefinition.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * EF Core entity representing a configured network connectivity test.
 * AI-assisted: entity property scaffolding; reviewed and directed by Ryan Loiselle.
 */

namespace HNW.Data.Models;

/// <summary>
/// A configured network connectivity test. One definition → one Quartz job.
/// </summary>
public class NetworkTestDefinition
{
    // ── IDENTITY ──────────────────────────────────────────────────────────────
    public Guid   Id          { get; set; } = Guid.NewGuid();
    public string Name        { get; set; } = string.Empty;   // human-readable label
    public string Destination { get; set; } = string.Empty;   // IP or FQDN

    // ── PROTOCOL ─────────────────────────────────────────────────────────────
    public NetworkServiceType ServiceType    { get; set; } = NetworkServiceType.TcpPort;
    public int?               Port           { get; set; }    // null → use service type default
    public string?            ExpectedStatus { get; set; }    // for HttpEndpoint: "200" etc.

    // ── SCHEDULE ─────────────────────────────────────────────────────────────
    // 5-part cron expression (no seconds). Minimum interval: */5 (every 5 min).
    public string CronExpression { get; set; } = "*/15 * * * *";
    public bool   IsEnabled      { get; set; } = true;

    // ── NETWORK POLICY (feature 007) ─────────────────────────────────────────
    public NetworkPolicyStatus PolicyStatus { get; set; } = NetworkPolicyStatus.Unknown;
    public string?             PolicyPrUrl  { get; set; }

    // ── AUDIT ─────────────────────────────────────────────────────────────────
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // ── NAVIGATION ───────────────────────────────────────────────────────────
    public ICollection<NetworkTestResult>      Results      { get; set; } = new List<NetworkTestResult>();
    public ICollection<NetworkTestStateChange> StateChanges { get; set; } = new List<NetworkTestStateChange>();

} // end NetworkTestDefinition

// ── ENUMS ─────────────────────────────────────────────────────────────────────

/// <summary>
/// The category of service being tested. Determines default port and probe handler.
/// </summary>
public enum NetworkServiceType
{
    TcpPort,        // Generic TCP — user supplies port
    HttpEndpoint,   // HTTP GET check — default 443
    DnsResolve,     // DNS resolution — default UDP 53
    NtpServer,      // NTP time sync — default UDP 123
    SmtpRelay,      // SMTP EHLO — default 587
    LdapServer,     // LDAP/LDAPS — default 636
    OidcProvider,   // GET /.well-known/openid-configuration — default 443
    DatabaseServer, // External database outside namespace (Oracle 1521, SQL Server 1433, PostgreSQL 5432, MySQL 3306) — user must specify port
    FileService,    // SMB/CIFS — default 445
    KubernetesApi,  // OCP API — default 6443
    CustomTcp       // Custom TCP — user supplies port and description
}

/// <summary>
/// Status of the automated NetworkPolicy egress rule for this test destination.
/// </summary>
public enum NetworkPolicyStatus
{
    Unknown,              // Policy check not yet run (GITHUB_TOKEN not set or first run)
    Covered,              // Matching egress policy already exists in tenant-gitops-be808f
    PrPending,            // PR opened; awaiting merge
    PrMerged,             // PR merged; policy is active
    ManuallyConfigured    // Operator marked as manually configured
}
