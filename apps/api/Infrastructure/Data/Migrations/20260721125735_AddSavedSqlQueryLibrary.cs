using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAHub.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedSqlQueryLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedSqlQueries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Statement = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSqlQueries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSqlQueries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSqlQueries_ProductId_Name",
                table: "SavedSqlQueries",
                columns: new[] { "ProductId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedSqlQueries");
        }
    }
}
