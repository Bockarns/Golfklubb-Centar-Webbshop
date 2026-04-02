using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golfklubb_Centar_Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemModelAndFullNameToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "CentarOrderMngt",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "CentarOrderMngt",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_OrderId = table.Column<int>(type: "int", nullable: false),
                    FK_ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemId", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_OrderId",
                        column: x => x.FK_OrderId,
                        principalSchema: "CentarOrderMngt",
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductId",
                        column: x => x.FK_ProductId,
                        principalSchema: "CentarProductMngt",
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_FK_OrderId",
                schema: "CentarOrderMngt",
                table: "OrderItems",
                column: "FK_OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_FK_ProductId",
                schema: "CentarOrderMngt",
                table: "OrderItems",
                column: "FK_ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "CentarOrderMngt");

            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "CentarOrderMngt",
                table: "Orders");
        }
    }
}
