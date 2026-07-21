using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompleteBugManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedBugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugRelations_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugRelations_Bugs_RelatedBugId",
                        column: x => x.RelatedBugId,
                        principalTable: "Bugs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugRelations_BugId_RelatedBugId",
                table: "BugRelations",
                columns: new[] { "BugId", "RelatedBugId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BugRelations_RelatedBugId",
                table: "BugRelations",
                column: "RelatedBugId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugRelations");
        }
    }
}
