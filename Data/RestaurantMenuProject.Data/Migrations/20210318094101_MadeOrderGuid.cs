using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class MadeOrderGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                  name: "Orders",
                  columns: table => new
                  {
                      Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                      ProcessType = table.Column<int>(type: "int", nullable: false),
                      DeliveredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                      PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                      ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                      PromoCodeId = table.Column<int>(type: "int", nullable: true),
                      CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                      ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                      IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                      DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                  },
                  constraints: table =>
                  {
                      table.PrimaryKey("PK_Orders", x => x.Id);
                      table.ForeignKey(
                          name: "FK_Orders_AspNetUsers_ClientId",
                          column: x => x.ClientId,
                          principalTable: "AspNetUsers",
                          principalColumn: "Id",
                          onDelete: ReferentialAction.Restrict);
                      table.ForeignKey(
                          name: "FK_Orders_PromoCodes_PromoCodeId",
                          column: x => x.PromoCodeId,
                          principalTable: "PromoCodes",
                          principalColumn: "Id",
                          onDelete: ReferentialAction.Restrict);
                  });

            migrationBuilder.CreateTable(
    name: "OrderDrinks",
    columns: table => new
    {
        OrderId = table.Column<int>(type: "nvarchar(450)", nullable: false),
        DrinkId = table.Column<string>(type: "nvarchar(450)", nullable: false),
        Count = table.Column<int>(type: "int", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_OrderDrinks", x => new { x.DrinkId, x.OrderId });
        table.ForeignKey(
            name: "FK_OrderDrinks_Drinks_DrinkId",
            column: x => x.DrinkId,
            principalTable: "Drinks",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
        table.ForeignKey(
            name: "FK_OrderDrinks_Orders_OrderId",
            column: x => x.OrderId,
            principalTable: "Orders",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    });
            migrationBuilder.CreateTable(
                name: "OrdersDishes",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "nvarchar(450)", nullable: false),
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersDishes", x => new { x.DishId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_OrdersDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdersDishes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDrinks_OrderId",
                table: "OrderDrinks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsDeleted",
                table: "Orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromoCodeId",
                table: "Orders",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDishes_OrderId",
                table: "OrdersDishes",
                column: "OrderId");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
    name: "OrderDrinks");

            migrationBuilder.DropTable(
                name: "OrdersDishes");

            migrationBuilder.DropTable(
    name: "Orders");
        }
    }
}
