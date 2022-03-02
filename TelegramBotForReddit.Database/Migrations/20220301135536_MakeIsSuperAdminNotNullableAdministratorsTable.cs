using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBotForReddit.Database.Migrations
{
    public partial class MakeIsSuperAdminNotNullableAdministratorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSuperAdmin",
                table: "Administrators",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSuperAdmin",
                table: "Administrators",
                type: "INTEGER",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);
        }
    }
}
