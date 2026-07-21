using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StartBugManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StepsToReproduce = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ActualResult = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reporter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Assignee = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FixBuildId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanonicalBugId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bugs_Bugs_CanonicalBugId",
                        column: x => x.CanonicalBugId,
                        principalTable: "Bugs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bugs_ProductBuilds_FixBuildId",
                        column: x => x.FixBuildId,
                        principalTable: "ProductBuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bugs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BugRunLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestRunAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugRunLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugRunLinks_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugRunLinks_TestRunAttempts_TestRunAttemptId",
                        column: x => x.TestRunAttemptId,
                        principalTable: "TestRunAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BugStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ChangedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugStatusHistory_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugRunLinks_BugId_TestRunAttemptId",
                table: "BugRunLinks",
                columns: new[] { "BugId", "TestRunAttemptId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BugRunLinks_TestRunAttemptId",
                table: "BugRunLinks",
                column: "TestRunAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_CanonicalBugId",
                table: "Bugs",
                column: "CanonicalBugId");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_FixBuildId",
                table: "Bugs",
                column: "FixBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_ProductId_Code",
                table: "Bugs",
                columns: new[] { "ProductId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_ProductId_Status_CreatedAtUtc",
                table: "Bugs",
                columns: new[] { "ProductId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BugStatusHistory_BugId_ChangedAtUtc",
                table: "BugStatusHistory",
                columns: new[] { "BugId", "ChangedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugRunLinks");

            migrationBuilder.DropTable(
                name: "BugStatusHistory");

            migrationBuilder.DropTable(
                name: "Bugs");
        }
    }
}
