namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class DrinkTypeServiceTests : BaseServiceTests
    {
        private IDrinkTypeService DrinkTypeService => this.ServiceProvider.GetRequiredService<IDrinkTypeService>();

        [Fact]
        public async Task GetAllDrinkTypesWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var expected = this.DbContext.DrinkTypes.To<MenuItemViewModel>();
            var actual = this.DrinkTypeService.GetAllDrinkTypes();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllDrinkTypesWithIdWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var expected = this.DbContext.DrinkTypes.To<FoodTypeViewModel>();

            var actual = this.DrinkTypeService.GetAllDrinkTypesWithId();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetDrinkTypeByIdWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var expected = this.DbContext.DrinkTypes.FirstOrDefault();
            var actual = this.DrinkTypeService.GetDrinkTypeById(expected.Id);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetDrinkTypeByIdReturnsNullWhenGivenInvalidId()
        {
            var DrinkType = this.DrinkTypeService.GetDrinkTypeById(999);

            Assert.Null(DrinkType);
        }

        [Fact]
        public async Task DeleteDrinkTypeAsyncWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var id = this.DbContext.DrinkTypes.FirstOrDefault().Id;

            var expectedCount = this.DbContext.DrinkTypes.Count() - 1;
            await this.DrinkTypeService.DeleteDrinkTypeAsync(id);

            var actualCount = this.DbContext.DrinkTypes.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task DeleteDrinkTypeAsyncThrowsExceptionWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DrinkTypeService.DeleteDrinkTypeAsync(9129312));
        }

        [Fact]
        public async Task AddDrinkTypeAsyncWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var expectedCount = 3;
            var actualCount = this.DbContext.DrinkTypes.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllDrinkTypesWithIdsWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();
            var DrinkNames = new string[] { "test1", "test2" };

            var expected = this.DbContext.DrinkTypes.Where(x => DrinkNames.Contains(x.Name));
            var ids = expected.Select(x => x.Id).ToArray();

            var actual = this.DrinkTypeService.GetAllDrinkTypesWithIds(ids);
            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public async Task EditDrinkTypeAsyncWorksCorrectly()
        {
            await this.AddDrinkTypesToDB();

            var DrinkType = this.DbContext.DrinkTypes.FirstOrDefault();
            var imageId = DrinkType.Image.Id;
            var editDrinkType = new EditCategoryViewModel()
            {
                Id = DrinkType.Id,
                Name = "test99",
                Description = "test999",
                NewImage = this.GetFile("testimage"),
            };
            var actual = this.DbContext.DrinkTypes.FirstOrDefault(x => x.Id == editDrinkType.Id);
            await this.DrinkTypeService.EditDrinkTypeAsync(editDrinkType, AppDomain.CurrentDomain.BaseDirectory);
            Assert.Equal(editDrinkType.Name, actual.Name);
            Assert.Equal(editDrinkType.Description, actual.Description);
            Assert.NotEqual(imageId, actual.Image.Id);
        }

        private async Task AddDrinkTypesToDB()
        {
            var drinkType1 = new AddCategoryViewModel()
            {
                Name = "test1",
                Description = "test1",
                Image = this.GetFile("test1"),
            };

            var drinkType2 = new AddCategoryViewModel()
            {
                Name = "test2",
                Description = "test2",
                Image = this.GetFile("test2"),
            };

            var drinkType3 = new AddCategoryViewModel()
            {
                Name = "test3",
                Description = "test3",
                Image = this.GetFile("test3"),
            };

            await this.DrinkTypeService.AddDrinkTypeAsync(drinkType1, AppDomain.CurrentDomain.BaseDirectory);
            await this.DrinkTypeService.AddDrinkTypeAsync(drinkType2, AppDomain.CurrentDomain.BaseDirectory);
            await this.DrinkTypeService.AddDrinkTypeAsync(drinkType3, AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
