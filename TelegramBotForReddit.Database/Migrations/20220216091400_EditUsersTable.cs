using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBotForReddit.Database.Migrations
{
    public partial class EditUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateStarted",
                table: "Users",
                type: "TEXT",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStopped",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateStarted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateStopped",
                table: "Users");
        }
    }
}
