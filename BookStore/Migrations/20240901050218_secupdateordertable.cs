using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class secupdateordertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_Orders", x => x.Id);
                    table.ForeignKey(
                            name: "FK_Orders_AspNetUsers_ClientId",
                            column: x => x.ClientId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                            );
                }
                );

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1,1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_OrderItems", x => x.Id);
                    table.ForeignKey(
                            name: "FK_OrderItems_Orders_OrderId",
                            column: x => x.OrderId,
                            principalTable: "Orders",
                            principalColumn: "Id");
                    table.ForeignKey(
                          name: "FK_OrderItems_Products_ProductId",
                          column: x => x.ProductId,
                          principalTable: "Products",
                          principalColumn: "Id",
                          onDelete: ReferentialAction.Cascade
                          );
                }
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
