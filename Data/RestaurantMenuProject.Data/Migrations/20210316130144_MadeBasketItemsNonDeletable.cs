using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class MadeBasketItemsNonDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasketsDrinks_IsDeleted",
                table: "BasketsDrinks");

            migrationBuilder.DropIndex(
                name: "IX_BasketsDishes_IsDeleted",
                table: "BasketsDishes");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "BasketsDrinks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BasketsDrinks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "BasketsDishes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BasketsDishes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "BasketsDrinks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BasketsDrinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "BasketsDishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BasketsDishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BasketsDrinks_IsDeleted",
                table: "BasketsDrinks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BasketsDishes_IsDeleted",
                table: "BasketsDishes",
                column: "IsDeleted");
        }
    }
}
