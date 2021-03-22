using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedPriceForOneInOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceForOne",
                table: "OrdersDishes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForOne",
                table: "OrderDrinks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceForOne",
                table: "OrdersDishes");

            migrationBuilder.DropColumn(
                name: "PriceForOne",
                table: "OrderDrinks");
        }
    }
}
