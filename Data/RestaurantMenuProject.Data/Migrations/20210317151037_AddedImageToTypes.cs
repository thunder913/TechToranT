using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedImageToTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "DrinkTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "DishTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DrinkTypes_ImageId",
                table: "DrinkTypes",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DishTypes_ImageId",
                table: "DishTypes",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishTypes_Images_ImageId",
                table: "DishTypes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DrinkTypes_Images_ImageId",
                table: "DrinkTypes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishTypes_Images_ImageId",
                table: "DishTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_DrinkTypes_Images_ImageId",
                table: "DrinkTypes");

            migrationBuilder.DropIndex(
                name: "IX_DrinkTypes_ImageId",
                table: "DrinkTypes");

            migrationBuilder.DropIndex(
                name: "IX_DishTypes_ImageId",
                table: "DishTypes");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "DrinkTypes");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "DishTypes");
        }
    }
}
