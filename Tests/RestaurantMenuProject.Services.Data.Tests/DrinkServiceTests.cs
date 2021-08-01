using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class DrinkServiceTests : BaseServiceTests
    {
        private IDrinkService DrinkService => this.ServiceProvider.GetRequiredService<IDrinkService>();

        [Fact]
        public async Task AddDrinkAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var ingredients = this.DbContext.Ingredients.Take(2).ToList();
            var ingredientsId = ingredients.Select(x => x.Id).ToList();

            var addDrinkViewModel = new AddDrinkViewModel()
            {
                Name = "newItem",
                AdditionalInfo = "test1",
                Weight = 50,
                Price = 10,
                Image = this.GetFile("test123"),
                DrinkTypeId = 1,
                IngredientsId = ingredientsId,
                AlchoholByVolume = 20,
                PackagingTypeId = 1,
            };
            await this.DrinkService.AddDrinkAsync(addDrinkViewModel, AppDomain.CurrentDomain.BaseDirectory);
            var newDrink = this.DbContext.Drinks.Include(x => x.Ingredients).FirstOrDefault(x => x.Name == "newItem");
            var expected = AutoMapperConfig.MapperInstance.Map<Drink>(addDrinkViewModel);
            expected.Ingredients = ingredients;
            Assert.Equal(expected.Name, newDrink.Name);
            Assert.Equal(expected.AdditionalInfo, newDrink.AdditionalInfo);
            Assert.Equal(expected.Weight, newDrink.Weight);
            Assert.Equal(expected.Price, newDrink.Price);
            Assert.Equal(ImageExtension.jpeg, newDrink.Image.Extension);
            Assert.Equal(expected.DrinkTypeId, newDrink.DrinkTypeId);
            expected.Ingredients.ShouldDeepEqual(newDrink.Ingredients);
            Assert.Equal(expected.AlchoholByVolume, newDrink.AlchoholByVolume);
            Assert.Equal(expected.PackagingTypeId, newDrink.PackagingTypeId);
        }

        [Fact]
        public async Task GetDrinkItemViewModelByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var expected = AutoMapperConfig.MapperInstance.Map<DrinkItemViewModel>(this.DbContext.Drinks.FirstOrDefault());
            var actual = this.DrinkService.GetDrinkItemViewModelById(expected.Id);

            actual.WithDeepEqual(expected)
                .IgnoreSourceProperty(x => x.DrinkType)
                .Assert();

            actual.DrinkType.WithDeepEqual(expected.DrinkType)
                .IgnoreSourceProperty(x => x.Drinks)
                .Assert();
        }

        [Fact]
        public void GetDrinkItemViewModelByIdReturnsNullWhenGivenInvalidId()
        {
            Assert.Null(this.DrinkService.GetDrinkItemViewModelById("INVALID!"));
        }

        [Fact]
        public async Task GetDrinkByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var drinkId = "test1";
            var expected = this.DbContext.Drinks.FirstOrDefault(x => x.Id == drinkId);
            var actual = this.DrinkService.GetDrinkById(drinkId);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetDrinkByIdReturnsNullWhenGivenInvalidId()
        {

            var actual = this.DrinkService.GetDrinkById("INVALID!");

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetAllDrinksByTypeWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var drinkTypeName = this.DbContext.DrinkTypes.FirstOrDefault(x => x.Id == 1).Name;

            var expected = this.DbContext.Drinks
                .Where(x => x.DrinkType.Name == drinkTypeName)
                .To<DrinkItemViewModel>()
                .ToList();

            var actual = this.DrinkService.GetAllDrinksByType(drinkTypeName).ToArray();

            for (int i = 0; i < actual.Count(); i++)
            {
                actual[i].WithDeepEqual(expected[i])
                    .IgnoreSourceProperty(x => x.DrinkType)
                    .Assert();

                actual[i].DrinkType.WithDeepEqual(expected[i].DrinkType)
                    .IgnoreSourceProperty(x => x.Drinks)
                    .Assert();
            }
        }

        [Fact]
        public async Task GetEditDrinkViewModelByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var drinkId = this.DbContext.Drinks.FirstOrDefault().Id;

            var expected = this.DbContext.Drinks
                .Where(x => x.Id == drinkId)
                .To<EditDrinkViewModel>()
                .FirstOrDefault();
            var actual = this.DrinkService.GetEditDrinkViewModelById(drinkId);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetEditDrinkViewModelByIdReturnsNullWhenTheGivenIdIsNull()
        {
            var drink = this.DrinkService.GetEditDrinkViewModelById("INVALID!");

            Assert.Null(drink);
        }

        [Fact]
        public async Task DeleteDrinkByIdAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var drinkId = this.DbContext.Drinks.FirstOrDefault().Id;
            await this.DrinkService.DeleteDrinkByIdAsync(drinkId);
            var drink = this.DbContext.Drinks.FirstOrDefault(x => x.Id == drinkId);

            Assert.Null(drink);
        }

        [Fact]
        public async Task DeleteDrinkByIdAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DrinkService.DeleteDrinkByIdAsync("INVALID"));
        }

        [Fact]
        public async Task GetDrinkWithDeletedByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var drink = this.DbContext.Drinks.FirstOrDefault();
            drink.IsDeleted = true;
            drink.DeletedOn = DateTime.UtcNow;
            await this.DbContext.SaveChangesAsync();

            var actual = this.DrinkService.GetDrinkWithDeletedById(drink.Id);
            actual.WithDeepEqual(drink)
                .IgnoreSourceProperty(x => x.Ingredients)
                .IgnoreSourceProperty(x => x.DrinkType)
                .IgnoreSourceProperty(x => x.PackagingType)
                .Assert();
        }

        [Fact]
        public void GetDrinkWithDeletedByIdReturnsNull()
        {
            var actual = this.DrinkService.GetDrinkWithDeletedById("INVALID!");
            Assert.Null(actual);
        }

        [Fact]
        public async Task EditDrinkAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDrinksToDB();

            var newIngredient = new Ingredient()
            {
                Name = "test3",
                Id = 3,
            };

            this.DbContext.Ingredients.Add(newIngredient);
            await this.DbContext.SaveChangesAsync();
            var ingredient = this.DbContext.Ingredients.FirstOrDefault(x => x.Id == 1);
            var drinkTypeId = 2;
            var ingredientId = 1;
            var packagingTypeId = 2;
            var image = this.GetFile("test123");
            var expectedName = "edit";
            var expectedIntValue = 33;
            var editDrink = new EditDrinkViewModel()
            {
                Id = "test1",
                Name = expectedName,
                Price = expectedIntValue,
                Weight = expectedIntValue,
                AdditionalInfo = expectedName,
                DrinkTypeId = drinkTypeId,
                IngredientsId = new List<int>() { ingredientId, newIngredient.Id },
                NewImage = image,
                AlchoholByVolume = expectedIntValue,
                PackagingTypeId = packagingTypeId,
            };

            await this.DrinkService.EditDrinkAsync(editDrink, AppDomain.CurrentDomain.BaseDirectory);

            var actualIngredient = this.DbContext.Ingredients.FirstOrDefault(x => x.Id == ingredientId);
            var actual = this.DbContext.Drinks.FirstOrDefault(x => x.Id == "test1");

            Assert.Equal(editDrink.Name, actual.Name);
            Assert.Equal(expectedIntValue, actual.Price);
            Assert.Equal(expectedIntValue, actual.Weight);
            Assert.Equal(expectedIntValue, actual.AlchoholByVolume);
            Assert.Equal(packagingTypeId, actual.PackagingTypeId);
            Assert.Equal(expectedName, actual.AdditionalInfo);
            Assert.Equal(drinkTypeId, actual.DrinkTypeId);
            Assert.Equal(new List<Ingredient>() { actualIngredient, newIngredient }, actual.Ingredients);
            Assert.Equal(ImageExtension.jpeg, actual.Image.Extension);
        }

        [Fact]
        public async Task EditDishAsyncThrowsExceptionWhenEditDishIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DrinkService.EditDrinkAsync(null, AppDomain.CurrentDomain.BaseDirectory));
        }

        private async Task PopulateDB()
        {
            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test1",
                Id = 1,
            });

            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test2",
                Id = 2,
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 2,
                Name = "test2",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 1,
                Name = "test1",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddDrinksToDB()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test1", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 1,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test2", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 2,
                });

            await this.DbContext.SaveChangesAsync();
        }

    }
}
