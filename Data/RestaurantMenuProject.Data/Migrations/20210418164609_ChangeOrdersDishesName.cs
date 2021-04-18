using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class ChangeOrdersDishesName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDishes_Dishes_DishId",
                table: "OrdersDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDishes_Orders_OrderId",
                table: "OrdersDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdersDishes",
                table: "OrdersDishes");

            migrationBuilder.RenameTable(
                name: "OrdersDishes",
                newName: "OrderDishes");

            migrationBuilder.RenameIndex(
                name: "IX_OrdersDishes_OrderId",
                table: "OrderDishes",
                newName: "IX_OrderDishes_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDishes",
                table: "OrderDishes",
                columns: new[] { "DishId", "OrderId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_Orders_OrderId",
                table: "OrderDishes",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_Orders_OrderId",
                table: "OrderDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDishes",
                table: "OrderDishes");

            migrationBuilder.RenameTable(
                name: "OrderDishes",
                newName: "OrdersDishes");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDishes_OrderId",
                table: "OrdersDishes",
                newName: "IX_OrdersDishes_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdersDishes",
                table: "OrdersDishes",
                columns: new[] { "DishId", "OrderId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDishes_Dishes_DishId",
                table: "OrdersDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDishes_Orders_OrderId",
                table: "OrdersDishes",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
