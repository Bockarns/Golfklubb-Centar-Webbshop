using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class AddForumBanAndProfileImagePathToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForumBanned",
                schema: "CentarUserMngt",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                schema: "CentarUserMngt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForumBanned",
                schema: "CentarUserMngt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                schema: "CentarUserMngt",
                table: "Users");
        }
    }
}
