using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HNW.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedNetworkReferenceLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReferenceLinks",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "IsEnvironmentRelative", "SortOrder", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "OpenShiftNetworking", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "How the OpenShift SDN plugin manages pod networking and inter-namespace isolation.", true, false, 10, "OpenShift SDN Overview", "https://docs.openshift.com/container-platform/4.14/networking/openshift_sdn/about-openshift-sdn.html" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "OpenShiftNetworking", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Overview of Kubernetes NetworkPolicy and how OpenShift enforces default-deny.", true, false, 20, "Network Policy in OpenShift", "https://docs.openshift.com/container-platform/4.14/networking/network_policy/about-network-policy.html" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "OpenShiftNetworking", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Step-by-step guide to creating NetworkPolicy objects for egress traffic.", true, false, 30, "Configuring egress NetworkPolicy", "https://docs.openshift.com/container-platform/4.14/networking/network_policy/creating-network-policy.html" },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "OpenShiftNetworking", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Platform-specific NetworkPolicy guidance for BC Gov OpenShift namespaces.", true, false, 40, "BC Gov Private Cloud — Network Policies", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/" },
                    { new Guid("11111111-1111-1111-1111-111111111105"), "OpenShiftNetworking", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "OVN-Kubernetes network provider used in newer OpenShift clusters including Emerald.", true, false, 50, "OVN-Kubernetes Overview", "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/about-ovn-kubernetes.html" },
                    { new Guid("22222222-2222-2222-2222-222222222201"), "ClusterTiers", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Comparison of Silver, Gold, and Emerald cluster capabilities, SDN implementations, and networking differences.", true, false, 10, "BC Gov Private Cloud Clusters", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters" },
                    { new Guid("22222222-2222-2222-2222-222222222202"), "ClusterTiers", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Silver-specific networking: OpenShift SDN, HAProxy router, ingress/egress defaults.", true, false, 20, "Silver Cluster Networking Notes", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#silver" },
                    { new Guid("22222222-2222-2222-2222-222222222203"), "ClusterTiers", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Gold-specific networking: OVN-Kubernetes, default-deny stance, multi-zone topology.", true, false, 30, "Gold Cluster Networking Notes", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#gold" },
                    { new Guid("22222222-2222-2222-2222-222222222204"), "ClusterTiers", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Emerald-specific networking: strict default-deny ingress AND egress, AVI load balancer, DataClass enforcement.", true, false, 40, "Emerald Cluster Networking Notes", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#emerald" },
                    { new Guid("22222222-2222-2222-2222-222222222205"), "ClusterTiers", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "AVI replaces HAProxy on Emerald. Routes must carry the correct AVI InfraSettings annotation.", true, false, 50, "AVI / NSX Advanced Load Balancer (Emerald)", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#avi-nsx-advanced-load-balancer" },
                    { new Guid("33333333-3333-3333-3333-333333333301"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "How BC Government classifies data (Low / Medium / High) and what each class means for network access.", true, false, 10, "BC Gov DataClass Overview", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification" },
                    { new Guid("33333333-3333-3333-3333-333333333302"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The three network zones and which data classes are permitted in each.", true, false, 20, "Network Zone Model (Public / Private / Restricted)", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones" },
                    { new Guid("33333333-3333-3333-3333-333333333303"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Required pod label DataClass: Medium and how it interacts with Emerald's AVI InfraSettings.", true, false, 30, "DataClass Labels on Pods and Routes", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#labels" },
                    { new Guid("33333333-3333-3333-3333-333333333304"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "How DataClass labelling determines which egress network zones your pods can reach on Emerald.", true, false, 40, "Impact of DataClass on Egress (Emerald)", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#emerald-impact" },
                    { new Guid("33333333-3333-3333-3333-333333333305"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Only dataclass-medium has a registered VIP on Emerald. Never use dataclass-low.", true, false, 50, "AVI InfraSettings — dataclass-medium vs dataclass-low", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/avi-infrasettings" },
                    { new Guid("44444444-4444-4444-4444-444444444401"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Every network flow requires TWO policies: ingress on the receiver AND egress on the sender.", true, false, 10, "Two-policy rule: ingress + egress", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#two-policy-rule" },
                    { new Guid("44444444-4444-4444-4444-444444444402"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "All pods need an explicit DNS egress policy (UDP 53 + TCP 53) on Emerald — not included by default.", true, false, 20, "DNS egress policy (UDP+TCP 53)", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#dns" },
                    { new Guid("44444444-4444-4444-4444-444444444403"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "How to write a CIDR-based egress policy to allow traffic to external systems or on-prem networks.", true, false, 30, "Allow egress to external IP / CIDR", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr" },
                    { new Guid("44444444-4444-4444-4444-444444444404"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Kubernetes reference: only open the specific port and protocol needed — never wildcard egress.", true, false, 40, "Least-privilege NetworkPolicy", "https://kubernetes.io/docs/concepts/services-networking/network-policies/#the-networkpolicy-resource" },
                    { new Guid("44444444-4444-4444-4444-444444444405"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Standard database and service ports used when writing egress NetworkPolicy rules.", true, false, 50, "Common port reference (Oracle 1521, MSSQL 1433, PG 5432, MySQL 3306)", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#common-ports" },
                    { new Guid("55555555-5555-5555-5555-555555555501"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Emerald enforces default-deny on BOTH ingress and egress. Every flow must be explicitly allowed.", true, false, 10, "Default-deny ingress and egress on Emerald", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#default-deny" },
                    { new Guid("55555555-5555-5555-5555-555555555502"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Guidance on SDN migration and the differences between OpenShift SDN, Calico, and OVN-Kubernetes.", true, false, 20, "Calico vs OVN-Kubernetes", "https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/migrate-from-openshift-sdn.html" },
                    { new Guid("55555555-5555-5555-5555-555555555503"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "How to view and inspect NetworkPolicy objects on a running cluster to diagnose connectivity failures.", true, false, 30, "Troubleshooting NetworkPolicy", "https://docs.openshift.com/container-platform/4.14/networking/network_policy/viewing-network-policy.html" },
                    { new Guid("55555555-5555-5555-5555-555555555504"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use oc debug to launch a temporary pod and test egress connectivity with curl, nc, or nslookup.", true, false, 40, "oc debug — testing connectivity from a pod", "https://docs.openshift.com/container-platform/4.14/support/troubleshooting/troubleshooting-network-issues.html" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111105"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222204"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333303"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333304"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333305"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444402"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444403"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444404"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444405"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555502"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555503"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555504"));
        }
    }
}
