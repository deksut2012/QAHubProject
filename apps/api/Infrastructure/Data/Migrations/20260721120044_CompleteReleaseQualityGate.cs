using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompleteReleaseQualityGate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeployedAtUtc",
                table: "Releases",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeploymentNotes",
                table: "Releases",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeploymentStatus",
                table: "Releases",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PostReleaseValidated",
                table: "Releases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostReleaseValidatedBy",
                table: "Releases",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReleaseKnownIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mitigation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseKnownIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseKnownIssues_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseKnownIssues_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseRequirements_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseRequirements_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseKnownIssues_BugId",
                table: "ReleaseKnownIssues",
                column: "BugId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseKnownIssues_ReleaseId_BugId",
                table: "ReleaseKnownIssues",
                columns: new[] { "ReleaseId", "BugId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseRequirements_ReleaseId_RequirementId",
                table: "ReleaseRequirements",
                columns: new[] { "ReleaseId", "RequirementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseRequirements_RequirementId",
                table: "ReleaseRequirements",
                column: "RequirementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseKnownIssues");

            migrationBuilder.DropTable(
                name: "ReleaseRequirements");

            migrationBuilder.DropColumn(
                name: "DeployedAtUtc",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "DeploymentNotes",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "DeploymentStatus",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "PostReleaseValidated",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "PostReleaseValidatedBy",
                table: "Releases");
        }
    }
}
