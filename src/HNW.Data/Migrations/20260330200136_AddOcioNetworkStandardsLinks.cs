using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HNW.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOcioNetworkStandardsLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReferenceLinks",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "IsEnvironmentRelative", "SortOrder", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333306"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Foundation policy: defines Protected A / B / C information classes. The OpenShift DataClass label (Low / Medium / High) maps directly to ISCF levels — Low = public info, Medium = Protected A / lower Protected B, High = Protected B-C.", true, false, 60, "BC Gov Information Security Classification Framework (ISCF)", "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification" },
                    { new Guid("33333333-3333-3333-3333-333333333307"), "DataClassAndZones", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Traditional OCIO zone model (IMIT Standard 6.13): Zone A = Restricted High Security (Protected B-C), Zone B = High Security (Protected A-low B), Zone C = Trusted Client (managed IDIR devices), DMZ = internet-facing proxies. SDN Low/Medium/High classifications map to these zones respectively.", true, false, 70, "BC Gov Network Security Zone Model — Zone A / B / C / DMZ", "https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security" },
                    { new Guid("44444444-4444-4444-4444-444444444406"), "NetworkPolicyPatterns", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Connections to external government partners (other ministries, Crown corps, health authorities) must traverse the ExtraNet zone via a Third Party Gateway (3PG). Requires formal approval and a dedicated egress NetworkPolicy rule targeting the 3PG CIDR.", true, false, 60, "Third Party Gateway (3PG) / ExtraNet — external partner connectivity", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/" },
                    { new Guid("55555555-5555-5555-5555-555555555505"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "2022 BC Gov SDN model: Low = public info (DMZ-equivalent, internet accessible), Medium = Protected A (Zone B-equivalent, no direct internet), High = Protected B-C (Zone A-equivalent, internet blocked at guardrail). DataClass pod label must match workload classification.", true, false, 50, "SDN Security Classification — Low / Medium / High workload model", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification" },
                    { new Guid("55555555-5555-5555-5555-555555555506"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Medium security workloads cannot reach the internet directly — must use the SSBC SDN Forward Proxy (HTTP/HTTPS only). High security workloads require Ministry ISO (MISO) exemption for any internet access. Direct internet egress from Medium/High is denied at the guardrail.", true, false, 60, "Medium/High workloads — internet egress via Forward Proxy only", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr" },
                    { new Guid("55555555-5555-5555-5555-555555555507"), "SdnGuidance", new DateTimeOffset(new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "BC Gov zone adjacency rule: communication is only permitted between adjacent zones. Traffic path: Internet → DMZ/Low → Medium → High. A session cannot be initiated directly from the internet into Medium or High zones. NetworkPolicy cannot bypass this adjacent-zones requirement.", true, false, 70, "Zone adjacency rule — no zone hopping", "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333306"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333307"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444406"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555505"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555506"));

            migrationBuilder.DeleteData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555507"));
        }
    }
}
