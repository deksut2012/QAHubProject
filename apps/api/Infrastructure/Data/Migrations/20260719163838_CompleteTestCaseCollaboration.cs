using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompleteTestCaseCollaboration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceTestCaseId",
                table: "TestCases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceVersionId",
                table: "TestCases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestCaseComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseComments_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseHistory_TestCaseVersions_TestCaseVersionId",
                        column: x => x.TestCaseVersionId,
                        principalTable: "TestCaseVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_SourceTestCaseId",
                table: "TestCases",
                column: "SourceTestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_SourceVersionId",
                table: "TestCases",
                column: "SourceVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseComments_TestCaseId_CreatedAtUtc",
                table: "TestCaseComments",
                columns: new[] { "TestCaseId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseHistory_TestCaseVersionId",
                table: "TestCaseHistory",
                column: "TestCaseVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_TestCaseVersions_SourceVersionId",
                table: "TestCases",
                column: "SourceVersionId",
                principalTable: "TestCaseVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_TestCases_SourceTestCaseId",
                table: "TestCases",
                column: "SourceTestCaseId",
                principalTable: "TestCases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_TestCaseVersions_SourceVersionId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_TestCases_SourceTestCaseId",
                table: "TestCases");

            migrationBuilder.DropTable(
                name: "TestCaseComments");

            migrationBuilder.DropTable(
                name: "TestCaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_TestCases_SourceTestCaseId",
                table: "TestCases");

            migrationBuilder.DropIndex(
                name: "IX_TestCases_SourceVersionId",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "SourceTestCaseId",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "SourceVersionId",
                table: "TestCases");
        }
    }
}
