using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedCommentsToDishesAndDrinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DishId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinkId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DishId",
                table: "Comments",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DrinkId",
                table: "Comments",
                column: "DrinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Dishes_DishId",
                table: "Comments",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Drinks_DrinkId",
                table: "Comments",
                column: "DrinkId",
                principalTable: "Drinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Dishes_DishId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Drinks_DrinkId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DishId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DrinkId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DrinkId",
                table: "Comments");
        }
    }
}
