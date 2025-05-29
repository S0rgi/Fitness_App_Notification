using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_App_Notification.Migrations
{
    /// <inheritdoc />
    public partial class InitNotificationPrefs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    Email = table.Column<string>(type: "text", nullable: false),
                    FriendInvite = table.Column<bool>(type: "boolean", nullable: false),
                    FriendResponse = table.Column<bool>(type: "boolean", nullable: false),
                    ChallengeInvite = table.Column<bool>(type: "boolean", nullable: false),
                    ChallengeResponse = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Email);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preferences");
        }
    }
}
