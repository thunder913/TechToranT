namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class DishTypeServiceTests : BaseServiceTests
    {
        private IDishTypeService DishTypeService => this.ServiceProvider.GetRequiredService<IDishTypeService>();

        [Fact]
        public async Task GetAllDishTypesWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.To<MenuItemViewModel>();

            var actual = this.DishTypeService.GetAllDishTypes();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllDishTypesWithIdWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.To<FoodTypeViewModel>();

            var actual = this.DishTypeService.GetAllDishTypesWithId();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishTypeByIdWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.FirstOrDefault();
            var actual = this.DishTypeService.GetDishTypeById(expected.Id);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetDishTypeByIdReturnsNullWhenGivenInvalidId()
        {
            var dishType = this.DishTypeService.GetDishTypeById(999);

            Assert.Null(dishType);
        }

        [Fact]
        public async Task DeleteDishTypeAsyncWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var id = this.DbContext.DishTypes.FirstOrDefault().Id;

            var expectedCount = this.DbContext.DishTypes.Count() - 1;
            await this.DishTypeService.DeleteDishTypeAsync(id);

            var actualCount = this.DbContext.DishTypes.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task DeleteDishTypeAsyncThrowsExceptionWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DishTypeService.DeleteDishTypeAsync(9129312));
        }

        [Fact]
        public async Task AddDishTypeAsyncWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expectedCount = 3;
            var actualCount = this.DbContext.DishTypes.Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetAllDishTypesWithIdsWorksCorrectly()
        {
            await this.AddDishTypesToDB();
            var dishNames = new string[] { "test1", "test2" };

            var expected = this.DbContext.DishTypes.Where(x => dishNames.Contains(x.Name));
            var ids = expected.Select(x => x.Id).ToArray();

            var actual = this.DishTypeService.GetAllDishTypesWithIds(ids);
            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public async Task EditDishTypeAsyncWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var dishType = this.DbContext.DishTypes.FirstOrDefault();
            var imageId = dishType.Image.Id;
            var editDishType = new EditCategoryViewModel()
            {
                Id = dishType.Id,
                Name = "test99",
                Description = "test999",
                NewImage = this.GetFile("testimage"),
            };
            var actual = this.DbContext.DishTypes.FirstOrDefault(x => x.Id == editDishType.Id);
            await this.DishTypeService.EditDishTypeAsync(editDishType, AppDomain.CurrentDomain.BaseDirectory);
            Assert.Equal(editDishType.Name, actual.Name);
            Assert.Equal(editDishType.Description, actual.Description);
            Assert.NotEqual(imageId, actual.Image.Id);
        }

        private async Task AddDishTypesToDB()
        {
            var dishtype1 = new AddCategoryViewModel()
            {
                Name = "test1",
                Description = "test1",
                Image = this.GetFile("test1"),
            };

            var dishtype2 = new AddCategoryViewModel()
            {
                Name = "test2",
                Description = "test2",
                Image = this.GetFile("test2"),
            };

            var dishtype3 = new AddCategoryViewModel()
            {
                Name = "test3",
                Description = "test3",
                Image = this.GetFile("test3"),
            };

            await this.DishTypeService.AddDishTypeAsync(dishtype1, AppDomain.CurrentDomain.BaseDirectory);
            await this.DishTypeService.AddDishTypeAsync(dishtype2, AppDomain.CurrentDomain.BaseDirectory);
            await this.DishTypeService.AddDishTypeAsync(dishtype3, AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
