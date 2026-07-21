using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StartReleaseQualityGate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RollbackPlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SignOffBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignOffReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SignedOffAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releases_ProductBuilds_BuildId",
                        column: x => x.BuildId,
                        principalTable: "ProductBuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_ProductEnvironments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "ProductEnvironments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_TestCycles_TestCycleId",
                        column: x => x.TestCycleId,
                        principalTable: "TestCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseChecklistItems_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseChecklistItems_ReleaseId_Code",
                table: "ReleaseChecklistItems",
                columns: new[] { "ReleaseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_BuildId",
                table: "Releases",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_EnvironmentId",
                table: "Releases",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ProductId_TargetDate",
                table: "Releases",
                columns: new[] { "ProductId", "TargetDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Releases_TestCycleId",
                table: "Releases",
                column: "TestCycleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseChecklistItems");

            migrationBuilder.DropTable(
                name: "Releases");
        }
    }
}
