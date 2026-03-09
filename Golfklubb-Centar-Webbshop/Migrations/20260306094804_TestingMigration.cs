using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class TestingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Testing",
                schema: "CentarUserMngt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Testing",
                schema: "CentarUserMngt",
                table: "Users");
        }
    }
}
