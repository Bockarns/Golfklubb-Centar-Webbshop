using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class AddedReviewReplyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewReplies",
                schema: "CentarProductMngt",
                columns: table => new
                {
                    ReviewReplyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplyContent = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    FK_ProductReviewId = table.Column<int>(type: "int", nullable: false),
                    FK_UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewReplyId", x => x.ReviewReplyId);
                    table.ForeignKey(
                        name: "FK_ReviewReplies_ProductReviewId",
                        column: x => x.FK_ProductReviewId,
                        principalSchema: "CentarProductMngt",
                        principalTable: "ProductReviews",
                        principalColumn: "ProductReviewId");
                    table.ForeignKey(
                        name: "FK_ReviewReplies_UserId",
                        column: x => x.FK_UserId,
                        principalSchema: "CentarUserMngt",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReplies_FK_ProductReviewId",
                schema: "CentarProductMngt",
                table: "ReviewReplies",
                column: "FK_ProductReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReplies_FK_UserId",
                schema: "CentarProductMngt",
                table: "ReviewReplies",
                column: "FK_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewReplies",
                schema: "CentarProductMngt");
        }
    }
}
