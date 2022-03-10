using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBotForReddit.Database.Migrations
{
    public partial class AddSubredditsTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subreddits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subreddits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subreddits_Name",
                table: "Subreddits",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subreddits");
        }
    }
}
