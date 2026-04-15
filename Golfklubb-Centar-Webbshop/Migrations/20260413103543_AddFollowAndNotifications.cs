using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Follows",
                schema: "CentarUserMngt",
                columns: table => new
                {
                    FollowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FK_FollowedUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FollowDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowId", x => x.FollowId);
                    table.ForeignKey(
                        name: "FK_Follows_FollowedUserId",
                        column: x => x.FK_FollowedUserId,
                        principalSchema: "CentarUserMngt",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Follows_UserId",
                        column: x => x.FK_UserId,
                        principalSchema: "CentarUserMngt",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "CentarUserMngt",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FK_CreatorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationId", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_CreatorUserId",
                        column: x => x.FK_CreatorUserId,
                        principalSchema: "CentarUserMngt",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_UserId",
                        column: x => x.FK_UserId,
                        principalSchema: "CentarUserMngt",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FK_FollowedUserId",
                schema: "CentarUserMngt",
                table: "Follows",
                column: "FK_FollowedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FK_UserId",
                schema: "CentarUserMngt",
                table: "Follows",
                column: "FK_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FK_CreatorUserId",
                schema: "CentarUserMngt",
                table: "Notifications",
                column: "FK_CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FK_UserId",
                schema: "CentarUserMngt",
                table: "Notifications",
                column: "FK_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Follows",
                schema: "CentarUserMngt");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "CentarUserMngt");
        }
    }
}
