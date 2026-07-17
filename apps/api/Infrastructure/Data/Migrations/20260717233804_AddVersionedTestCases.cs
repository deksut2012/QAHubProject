using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionedTestCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequirementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentVersionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCases_ProductModules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "ProductModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCases_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preconditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseVersions_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TestData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseSteps_TestCaseVersions_TestCaseVersionId",
                        column: x => x.TestCaseVersionId,
                        principalTable: "TestCaseVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_ModuleId",
                table: "TestCases",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_ProductId_Code",
                table: "TestCases",
                columns: new[] { "ProductId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_RequirementId",
                table: "TestCases",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseSteps_TestCaseVersionId_Sequence",
                table: "TestCaseSteps",
                columns: new[] { "TestCaseVersionId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseVersions_TestCaseId_VersionNumber",
                table: "TestCaseVersions",
                columns: new[] { "TestCaseId", "VersionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseSteps");

            migrationBuilder.DropTable(
                name: "TestCaseVersions");

            migrationBuilder.DropTable(
                name: "TestCases");
        }
    }
}
