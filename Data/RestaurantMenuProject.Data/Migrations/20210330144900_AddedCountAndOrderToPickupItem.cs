using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedCountAndOrderToPickupItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "PickupItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "PickupItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickupItems_OrderId",
                table: "PickupItems",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickupItems_Orders_OrderId",
                table: "PickupItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickupItems_Orders_OrderId",
                table: "PickupItems");

            migrationBuilder.DropIndex(
                name: "IX_PickupItems_OrderId",
                table: "PickupItems");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "PickupItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PickupItems");
        }
    }
}
