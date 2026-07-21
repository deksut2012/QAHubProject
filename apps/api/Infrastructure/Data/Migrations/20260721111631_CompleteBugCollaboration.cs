using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompleteBugCollaboration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VerificationAttemptId",
                table: "Bugs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BugComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugComments_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BugEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UploadedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugEvidence_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_VerificationAttemptId",
                table: "Bugs",
                column: "VerificationAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_BugComments_BugId_CreatedAtUtc",
                table: "BugComments",
                columns: new[] { "BugId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BugEvidence_BugId_UploadedAtUtc",
                table: "BugEvidence",
                columns: new[] { "BugId", "UploadedAtUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_TestRunAttempts_VerificationAttemptId",
                table: "Bugs",
                column: "VerificationAttemptId",
                principalTable: "TestRunAttempts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_TestRunAttempts_VerificationAttemptId",
                table: "Bugs");

            migrationBuilder.DropTable(
                name: "BugComments");

            migrationBuilder.DropTable(
                name: "BugEvidence");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_VerificationAttemptId",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "VerificationAttemptId",
                table: "Bugs");
        }
    }
}
