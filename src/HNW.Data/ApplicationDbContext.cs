/*
 * ApplicationDbContext.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * EF Core DbContext for HelloNetworkWorld. Manages all entities:
 * NetworkTestDefinitions, NetworkTestResults, NetworkTestStateChanges, ReferenceLinks.
 * AI-assisted: entity configuration scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HNW.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // ── ENTITY SETS ──────────────────────────────────────────────────────────
    public DbSet<NetworkTestDefinition> NetworkTestDefinitions => Set<NetworkTestDefinition>();
    public DbSet<NetworkTestResult>     NetworkTestResults     => Set<NetworkTestResult>();
    public DbSet<NetworkTestStateChange> NetworkTestStateChanges => Set<NetworkTestStateChange>();
    public DbSet<ReferenceLink>         ReferenceLinks         => Set<ReferenceLink>();

    // ── CONFIGURATION ────────────────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NetworkTestDefinition
        modelBuilder.Entity<NetworkTestDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Destination).HasMaxLength(253).IsRequired();
            e.Property(x => x.CronExpression).HasMaxLength(100).IsRequired();
            e.Property(x => x.ServiceType).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.PolicyStatus).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.PolicyPrUrl).HasMaxLength(500);
            e.HasMany(x => x.Results).WithOne(r => r.NetworkTestDefinition)
             .HasForeignKey(r => r.NetworkTestDefinitionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.StateChanges).WithOne(s => s.NetworkTestDefinition)
             .HasForeignKey(s => s.NetworkTestDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        // NetworkTestResult — index on ExecutedAt and FK for trendline performance
        modelBuilder.Entity<NetworkTestResult>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ExecutedAt);
            e.HasIndex(x => x.NetworkTestDefinitionId);
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
        });

        // NetworkTestStateChange
        modelBuilder.Entity<NetworkTestStateChange>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ChangedAt);
        });

        // ReferenceLink
        modelBuilder.Entity<ReferenceLink>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
        });

        // ── NETWORK REFERENCE LINKS SEED ─────────────────────────────────────
        // Stable GUIDs ensure idempotency across restarts. Never change these IDs.
        var seedDate = new DateTimeOffset(2026, 3, 30, 0, 0, 0, TimeSpan.Zero);
        modelBuilder.Entity<ReferenceLink>().HasData(
            // ── OpenShift Networking ──────────────────────────────────────────
            new ReferenceLink { Id = new Guid("11111111-1111-1111-1111-111111111101"), Category = ReferenceLinkCategory.OpenShiftNetworking, SortOrder = 10, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "OpenShift SDN Overview", Url = "https://docs.openshift.com/container-platform/4.14/networking/openshift_sdn/about-openshift-sdn.html", Description = "How the OpenShift SDN plugin manages pod networking and inter-namespace isolation." },
            new ReferenceLink { Id = new Guid("11111111-1111-1111-1111-111111111102"), Category = ReferenceLinkCategory.OpenShiftNetworking, SortOrder = 20, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Network Policy in OpenShift", Url = "https://docs.openshift.com/container-platform/4.14/networking/network_policy/about-network-policy.html", Description = "Overview of Kubernetes NetworkPolicy and how OpenShift enforces default-deny." },
            new ReferenceLink { Id = new Guid("11111111-1111-1111-1111-111111111103"), Category = ReferenceLinkCategory.OpenShiftNetworking, SortOrder = 30, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Configuring egress NetworkPolicy", Url = "https://docs.openshift.com/container-platform/4.14/networking/network_policy/creating-network-policy.html", Description = "Step-by-step guide to creating NetworkPolicy objects for egress traffic." },
            new ReferenceLink { Id = new Guid("11111111-1111-1111-1111-111111111104"), Category = ReferenceLinkCategory.OpenShiftNetworking, SortOrder = 40, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "BC Gov Private Cloud \u2014 Network Policies", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/", Description = "Platform-specific NetworkPolicy guidance for BC Gov OpenShift namespaces." },
            new ReferenceLink { Id = new Guid("11111111-1111-1111-1111-111111111105"), Category = ReferenceLinkCategory.OpenShiftNetworking, SortOrder = 50, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "OVN-Kubernetes Overview", Url = "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/about-ovn-kubernetes.html", Description = "OVN-Kubernetes network provider used in newer OpenShift clusters including Emerald." },
            // ── Cluster Tiers ─────────────────────────────────────────────────
            new ReferenceLink { Id = new Guid("22222222-2222-2222-2222-222222222201"), Category = ReferenceLinkCategory.ClusterTiers, SortOrder = 10, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "BC Gov Private Cloud Clusters", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters", Description = "Comparison of Silver, Gold, and Emerald cluster capabilities, SDN implementations, and networking differences." },
            new ReferenceLink { Id = new Guid("22222222-2222-2222-2222-222222222202"), Category = ReferenceLinkCategory.ClusterTiers, SortOrder = 20, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Silver Cluster Networking Notes", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#silver", Description = "Silver-specific networking: OpenShift SDN, HAProxy router, ingress/egress defaults." },
            new ReferenceLink { Id = new Guid("22222222-2222-2222-2222-222222222203"), Category = ReferenceLinkCategory.ClusterTiers, SortOrder = 30, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Gold Cluster Networking Notes", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#gold", Description = "Gold-specific networking: OVN-Kubernetes, default-deny stance, multi-zone topology." },
            new ReferenceLink { Id = new Guid("22222222-2222-2222-2222-222222222204"), Category = ReferenceLinkCategory.ClusterTiers, SortOrder = 40, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Emerald Cluster Networking Notes", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#emerald", Description = "Emerald-specific networking: strict default-deny ingress AND egress, AVI load balancer, DataClass enforcement." },
            new ReferenceLink { Id = new Guid("22222222-2222-2222-2222-222222222205"), Category = ReferenceLinkCategory.ClusterTiers, SortOrder = 50, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "AVI / NSX Advanced Load Balancer (Emerald)", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#avi-nsx-advanced-load-balancer", Description = "AVI replaces HAProxy on Emerald. Routes must carry the correct AVI InfraSettings annotation." },
            // ── DataClass & Network Zones ──────────────────────────────────────
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333301"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 10, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "BC Gov DataClass Overview", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification", Description = "How BC Government classifies data (Low / Medium / High) and what each class means for network access." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333302"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 20, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Network Zone Model (Public / Private / Restricted)", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones", Description = "The three network zones and which data classes are permitted in each." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333303"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 30, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "DataClass Labels on Pods and Routes", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#labels", Description = "Required pod label DataClass: Medium and how it interacts with Emerald's AVI InfraSettings." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333304"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 40, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Impact of DataClass on Egress (Emerald)", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#emerald-impact", Description = "How DataClass labelling determines which egress network zones your pods can reach on Emerald." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333305"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 50, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "AVI InfraSettings \u2014 dataclass-medium vs dataclass-low", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/avi-infrasettings", Description = "Only dataclass-medium has a registered VIP on Emerald. Never use dataclass-low." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333306"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 60, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "BC Gov Information Security Classification Framework (ISCF)", Url = "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification", Description = "Foundation policy: defines Protected A / B / C information classes. The OpenShift DataClass label (Low / Medium / High) maps directly to ISCF levels \u2014 Low = public info, Medium = Protected A / lower Protected B, High = Protected B-C." },
            new ReferenceLink { Id = new Guid("33333333-3333-3333-3333-333333333307"), Category = ReferenceLinkCategory.DataClassAndZones, SortOrder = 70, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "BC Gov Network Security Zone Model \u2014 Zone A / B / C / DMZ", Url = "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security", Description = "Traditional OCIO zone model (IMIT Standard 6.13): Zone A = Restricted High Security (Protected B-C), Zone B = High Security (Protected A-low B), Zone C = Trusted Client (managed IDIR devices), DMZ = internet-facing proxies. SDN Low/Medium/High classifications map to these zones respectively." },
            // ── NetworkPolicy Patterns ─────────────────────────────────────────
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444401"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 10, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Two-policy rule: ingress + egress", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#two-policy-rule", Description = "Every network flow requires TWO policies: ingress on the receiver AND egress on the sender." },
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444402"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 20, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "DNS egress policy (UDP+TCP 53)", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#dns", Description = "All pods need an explicit DNS egress policy (UDP 53 + TCP 53) on Emerald \u2014 not included by default." },
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444403"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 30, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Allow egress to external IP / CIDR", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr", Description = "How to write a CIDR-based egress policy to allow traffic to external systems or on-prem networks." },
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444404"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 40, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Least-privilege NetworkPolicy", Url = "https://kubernetes.io/docs/concepts/services-networking/network-policies/#the-networkpolicy-resource", Description = "Kubernetes reference: only open the specific port and protocol needed \u2014 never wildcard egress." },
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444405"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 50, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Common port reference (Oracle 1521, MSSQL 1433, PG 5432, MySQL 3306)", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#common-ports", Description = "Standard database and service ports used when writing egress NetworkPolicy rules." },
            new ReferenceLink { Id = new Guid("44444444-4444-4444-4444-444444444406"), Category = ReferenceLinkCategory.NetworkPolicyPatterns, SortOrder = 60, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Third Party Gateway (3PG) / ExtraNet \u2014 external partner connectivity", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/", Description = "Connections to external government partners (other ministries, Crown corps, health authorities) must traverse the ExtraNet zone via a Third Party Gateway (3PG). Requires formal approval and a dedicated egress NetworkPolicy rule targeting the 3PG CIDR." },
            // ── SDN Guidance ──────────────────────────────────────────────────
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555501"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 10, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Default-deny ingress and egress on Emerald", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#default-deny", Description = "Emerald enforces default-deny on BOTH ingress and egress. Every flow must be explicitly allowed." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555502"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 20, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Calico vs OVN-Kubernetes", Url = "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/migrate-from-openshift-sdn.html", Description = "Guidance on SDN migration and the differences between OpenShift SDN, Calico, and OVN-Kubernetes." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555503"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 30, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Troubleshooting NetworkPolicy", Url = "https://docs.openshift.com/container-platform/4.14/networking/network_policy/viewing-network-policy.html", Description = "How to view and inspect NetworkPolicy objects on a running cluster to diagnose connectivity failures." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555504"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 40, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "oc debug \u2014 testing connectivity from a pod", Url = "https://docs.openshift.com/container-platform/4.14/support/troubleshooting/troubleshooting-network-issues.html", Description = "Use oc debug to launch a temporary pod and test egress connectivity with curl, nc, or nslookup." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555505"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 50, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "SDN Security Classification \u2014 Low / Medium / High workload model", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification", Description = "2022 BC Gov SDN model: Low = public info (DMZ-equivalent, internet accessible), Medium = Protected A (Zone B-equivalent, no direct internet), High = Protected B-C (Zone A-equivalent, internet blocked at guardrail). DataClass pod label must match workload classification." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555506"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 60, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Medium/High workloads \u2014 internet egress via Forward Proxy only", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr", Description = "Medium security workloads cannot reach the internet directly \u2014 must use the SSBC SDN Forward Proxy (HTTP/HTTPS only). High security workloads require Ministry ISO (MISO) exemption for any internet access. Direct internet egress from Medium/High is denied at the guardrail." },
            new ReferenceLink { Id = new Guid("55555555-5555-5555-5555-555555555507"), Category = ReferenceLinkCategory.SdnGuidance, SortOrder = 70, IsActive = true, IsEnvironmentRelative = false, CreatedAt = seedDate, Title = "Zone adjacency rule \u2014 no zone hopping", Url = "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones", Description = "BC Gov zone adjacency rule: communication is only permitted between adjacent zones. Traffic path: Internet \u2192 DMZ/Low \u2192 Medium \u2192 High. A session cannot be initiated directly from the internet into Medium or High zones. NetworkPolicy cannot bypass this adjacent-zones requirement." }
        );
    }

} // end ApplicationDbContext
