using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantMenuProject.Data.Migrations
{
    public partial class AddedTypeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoCodeType",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "DrinkType",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "PackagingType",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "DishType",
                table: "Dishes");

            migrationBuilder.AddColumn<int>(
                name: "DrinkTypeId",
                table: "Drinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackagingTypeId",
                table: "Drinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DishTypeId",
                table: "Dishes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DishTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrinkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackagingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEco = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackagingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishTypePromoCode",
                columns: table => new
                {
                    PromoCodesId = table.Column<int>(type: "int", nullable: false),
                    ValidDishCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTypePromoCode", x => new { x.PromoCodesId, x.ValidDishCategoriesId });
                    table.ForeignKey(
                        name: "FK_DishTypePromoCode_DishTypes_ValidDishCategoriesId",
                        column: x => x.ValidDishCategoriesId,
                        principalTable: "DishTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishTypePromoCode_PromoCodes_PromoCodesId",
                        column: x => x.PromoCodesId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrinkTypePromoCode",
                columns: table => new
                {
                    PromoCodesId = table.Column<int>(type: "int", nullable: false),
                    ValidDrinkCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkTypePromoCode", x => new { x.PromoCodesId, x.ValidDrinkCategoriesId });
                    table.ForeignKey(
                        name: "FK_DrinkTypePromoCode_DrinkTypes_ValidDrinkCategoriesId",
                        column: x => x.ValidDrinkCategoriesId,
                        principalTable: "DrinkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrinkTypePromoCode_PromoCodes_PromoCodesId",
                        column: x => x.PromoCodesId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drinks_DrinkTypeId",
                table: "Drinks",
                column: "DrinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Drinks_PackagingTypeId",
                table: "Drinks",
                column: "PackagingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_DishTypeId",
                table: "Dishes",
                column: "DishTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DishTypePromoCode_ValidDishCategoriesId",
                table: "DishTypePromoCode",
                column: "ValidDishCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_DishTypes_IsDeleted",
                table: "DishTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkTypePromoCode_ValidDrinkCategoriesId",
                table: "DrinkTypePromoCode",
                column: "ValidDrinkCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkTypes_IsDeleted",
                table: "DrinkTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PackagingTypes_IsDeleted",
                table: "PackagingTypes",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_DishTypes_DishTypeId",
                table: "Dishes",
                column: "DishTypeId",
                principalTable: "DishTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drinks_DrinkTypes_DrinkTypeId",
                table: "Drinks",
                column: "DrinkTypeId",
                principalTable: "DrinkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drinks_PackagingTypes_PackagingTypeId",
                table: "Drinks",
                column: "PackagingTypeId",
                principalTable: "PackagingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_DishTypes_DishTypeId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Drinks_DrinkTypes_DrinkTypeId",
                table: "Drinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Drinks_PackagingTypes_PackagingTypeId",
                table: "Drinks");

            migrationBuilder.DropTable(
                name: "DishTypePromoCode");

            migrationBuilder.DropTable(
                name: "DrinkTypePromoCode");

            migrationBuilder.DropTable(
                name: "PackagingTypes");

            migrationBuilder.DropTable(
                name: "DishTypes");

            migrationBuilder.DropTable(
                name: "DrinkTypes");

            migrationBuilder.DropIndex(
                name: "IX_Drinks_DrinkTypeId",
                table: "Drinks");

            migrationBuilder.DropIndex(
                name: "IX_Drinks_PackagingTypeId",
                table: "Drinks");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_DishTypeId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "DrinkTypeId",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "PackagingTypeId",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "DishTypeId",
                table: "Dishes");

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeType",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DrinkType",
                table: "Drinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackagingType",
                table: "Drinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DishType",
                table: "Dishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
