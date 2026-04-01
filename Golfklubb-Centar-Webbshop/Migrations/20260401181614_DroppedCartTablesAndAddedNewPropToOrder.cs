using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class DroppedCartTablesAndAddedNewPropToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CartId",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "CentarOrderMngt");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "CentarOrderMngt");

            //migrationBuilder.DropIndex(
            //    name: "IX_Orders_FK_CartId",
            //    schema: "CentarOrderMngt",
            //    table: "Orders");

            migrationBuilder.DropColumn(
                name: "FK_CartId",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "CentarOrderMngt",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "FK_CartId",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "CentarOrderMngt",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPrice = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartId", x => x.CartId);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "CentarOrderMngt",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_CartId = table.Column<int>(type: "int", nullable: false),
                    FK_ProductId = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemId", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_CartId",
                        column: x => x.FK_CartId,
                        principalSchema: "CentarOrderMngt",
                        principalTable: "Carts",
                        principalColumn: "CartId");
                    table.ForeignKey(
                        name: "FK_CartItems_ProductId",
                        column: x => x.FK_ProductId,
                        principalSchema: "CentarProductMngt",
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FK_CartId",
                schema: "CentarOrderMngt",
                table: "Orders",
                column: "FK_CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_FK_CartId",
                schema: "CentarOrderMngt",
                table: "CartItems",
                column: "FK_CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_FK_ProductId",
                schema: "CentarOrderMngt",
                table: "CartItems",
                column: "FK_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CartId",
                schema: "CentarOrderMngt",
                table: "Orders",
                column: "FK_CartId",
                principalSchema: "CentarOrderMngt",
                principalTable: "Carts",
                principalColumn: "CartId");
        }
    }
}
