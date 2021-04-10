using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedPromoCodeToBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromoCodeId",
                table: "Baskets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_PromoCodeId",
                table: "Baskets",
                column: "PromoCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_PromoCodes_PromoCodeId",
                table: "Baskets",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_PromoCodes_PromoCodeId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_PromoCodeId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "PromoCodeId",
                table: "Baskets");
        }
    }
}
