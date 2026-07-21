using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StartTestExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductBuilds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBuilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBuilds_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Assignee = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCycles_ProductBuilds_BuildId",
                        column: x => x.BuildId,
                        principalTable: "ProductBuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCycles_ProductEnvironments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "ProductEnvironments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCycles_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCycleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Assignee = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCycleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCycleItems_TestCaseVersions_TestCaseVersionId",
                        column: x => x.TestCaseVersionId,
                        principalTable: "TestCaseVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCycleItems_TestCycles_TestCycleId",
                        column: x => x.TestCycleId,
                        principalTable: "TestCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRunAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCycleItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActualResult = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Evidence = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExecutedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRunAttempts_TestCycleItems_TestCycleItemId",
                        column: x => x.TestCycleItemId,
                        principalTable: "TestCycleItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBuilds_ProductId_Version",
                table: "ProductBuilds",
                columns: new[] { "ProductId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCycleItems_TestCaseVersionId",
                table: "TestCycleItems",
                column: "TestCaseVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCycleItems_TestCycleId_TestCaseVersionId",
                table: "TestCycleItems",
                columns: new[] { "TestCycleId", "TestCaseVersionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCycles_BuildId",
                table: "TestCycles",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCycles_EnvironmentId",
                table: "TestCycles",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCycles_ProductId",
                table: "TestCycles",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunAttempts_TestCycleItemId_AttemptNumber",
                table: "TestRunAttempts",
                columns: new[] { "TestCycleItemId", "AttemptNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRunAttempts");

            migrationBuilder.DropTable(
                name: "TestCycleItems");

            migrationBuilder.DropTable(
                name: "TestCycles");

            migrationBuilder.DropTable(
                name: "ProductBuilds");
        }
    }
}
