using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class AdditionTablesAndRelationsToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Flyttat innehåll till migrationremovedcontent.txt
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "CentarForumMngt");

            migrationBuilder.DropTable(
                name: "Histories",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "InvoiceItems",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "ProductReviews",
                schema: "CentarProductMngt");

            migrationBuilder.DropTable(
                name: "Stocks",
                schema: "CentarProductMngt");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "CentarForumMngt");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "CentarProductMngt");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Taxes",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "CentarProductMngt");

            migrationBuilder.DropTable(
                name: "Discounts",
                schema: "CentarProductMngt");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "CentarOrderMngt");
        }
    }
}
