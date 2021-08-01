namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class IngredientServiceTests : BaseServiceTests
    {
        private IIngredientService IngredientService => this.ServiceProvider.GetRequiredService<IIngredientService>();

        [Fact]
        public async Task AddIngredientAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var addIngredient = new AddIngredientViewModel()
            {
                Name = "newIngredient",
                AllergensId = new List<int>() { 1, 2, },
            };
            await this.IngredientService.AddIngredientAsync(addIngredient);

            var ingredient = this.DbContext.Ingredients.FirstOrDefault(x => x.Name == "newIngredient");
            var expectedAllergenCount = 2;
            Assert.NotNull(ingredient);
            Assert.Equal(expectedAllergenCount, ingredient.Allergens.Count);
        }

        [Fact]
        public async Task GetAllAsDishIngredientViewModelWorksCorrectly()
        {
            await this.PopulateDB();

            var expected = this.DbContext.Ingredients.To<DishIngredientViewModel>().ToList();
            var actual = this.IngredientService.GetAllAsDishIngredientViewModel();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllIngredientsByIdsWorksCorrectly()
        {
            await this.PopulateDB();

            var ids = this.DbContext.Ingredients.Take(2).Select(x => x.Id).ToArray();
            var expected = this.DbContext.Ingredients.Where(x => ids.Contains(x.Id)).ToList();
            var actual = this.IngredientService.GetAllIngredientsByIds(ids);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetIngredientByIdWorksCorrectly()
        {
            await this.PopulateDB();

            var expected = this.DbContext.Ingredients.Skip(1).FirstOrDefault();
            var actual = this.IngredientService.GetIngredientById(expected.Id);

            actual.ShouldDeepEqual(expected);
        }

        private async Task PopulateDB()
        {
            var allergen1 = new Allergen()
            {
                Id = 1,
                Name = "allergen1",
            };
            var allergen2 = new Allergen()
            {
                Id = 2,
                Name = "allergen2",
            };

            await this.DbContext.Allergens.AddAsync(allergen1);
            await this.DbContext.Allergens.AddAsync(allergen2);

            var ingredient1 = new Ingredient()
            {
                Id = 1,
                Name = "ingredient1",
                Allergens = new List<Allergen>() { allergen1 },
            };
            var ingredient2 = new Ingredient()
            {
                Id = 2,
                Name = "ingredient2",
                Allergens = new List<Allergen>() { allergen2 },
            };
            var ingredient3 = new Ingredient()
            {
                Id = 3,
                Name = "ingredient3",
                Allergens = new List<Allergen>() { allergen1, allergen2, },
            };

            await this.DbContext.Ingredients.AddAsync(ingredient1);
            await this.DbContext.Ingredients.AddAsync(ingredient2);
            await this.DbContext.Ingredients.AddAsync(ingredient3);

            await this.DbContext.SaveChangesAsync();
        }
    }
}
