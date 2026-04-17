using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class CityAndCountryAddedToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImagePath",
                schema: "CentarUserMngt",
                table: "Users",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "CentarUserMngt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "CentarUserMngt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "CentarUserMngt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "CentarUserMngt",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Country",
                schema: "CentarUserMngt",
                table: "Users",
                newName: "ProfileImagePath");
        }
    }
}
