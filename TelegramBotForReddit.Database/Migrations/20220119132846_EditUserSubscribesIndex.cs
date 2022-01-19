using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBotForReddit.Database.Migrations
{
    public partial class EditUserSubscribesIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSubscribes_SubredditName",
                table: "UserSubscribes");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscribes_SubredditName_UserId",
                table: "UserSubscribes",
                columns: new[] { "SubredditName", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSubscribes_SubredditName_UserId",
                table: "UserSubscribes");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscribes_SubredditName",
                table: "UserSubscribes",
                column: "SubredditName");
        }
    }
}
