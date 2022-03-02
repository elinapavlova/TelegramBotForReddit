using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBotForReddit.Database.Migrations
{
    public partial class AddIsSuperAdminFieldToAdministratorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "Administrators",
                type: "INTEGER",
                nullable: true,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "Administrators");
        }
    }
}
