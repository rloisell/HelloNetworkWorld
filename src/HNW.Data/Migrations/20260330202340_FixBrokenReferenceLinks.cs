using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNW.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBrokenReferenceLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#silver");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#gold");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222204"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/platform-network-topology/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333303"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333304"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333305"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444402"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444403"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444405"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444406"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/#default-policy");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555505"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/hosting-tiers-table/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555506"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555507"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/platform-network-topology/");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#silver");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#gold");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222204"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#emerald");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#avi-nsx-advanced-load-balancer");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333303"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#labels");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333304"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#emerald-impact");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333305"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/avi-infrasettings");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#two-policy-rule");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444402"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#dns");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444403"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444405"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#common-ports");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444406"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#default-deny");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555505"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555506"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr");

            migrationBuilder.UpdateData(
                table: "ReferenceLinks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555507"),
                column: "Url",
                value: "https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones");
        }
    }
}
