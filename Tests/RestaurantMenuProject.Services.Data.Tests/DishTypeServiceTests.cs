using DeepEqual.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class DishTypeServiceTests : BaseServiceTests
    {
        private IDishTypeService DishTypeService => this.ServiceProvider.GetRequiredService<IDishTypeService>();

        [Fact]
        public async Task GetAllDishTypesWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.To<MenuItemViewModel>();

            var actual = this.DishTypeService.GetAllDishTypes();

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllDishTypesWithIdWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.To<FoodTypeViewModel>();

            var actual = this.DishTypeService.GetAllDishTypesWithId();

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishTypeByIdWorksCorrectly()
        {
            await this.AddDishTypesToDB();

            var expected = this.DbContext.DishTypes.FirstOrDefault();
            var actual = this.DishTypeService.GetDishTypeById(expected.Id);

            actual.IsDeepEqual(expected);
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

        private IFormFile GetFile(string name)
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = $"{name}.jpeg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var file = fileMock.Object;

            return file;
        }
    }
}
