using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedPromoCodePercent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromoPercent",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoPercent",
                table: "PromoCodes");
        }
    }
}
