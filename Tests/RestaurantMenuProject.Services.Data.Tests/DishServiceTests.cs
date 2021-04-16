namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class DishServiceTests : BaseServiceTests
    {
        private IDishService DishService => this.ServiceProvider.GetRequiredService<IDishService>();

        [Fact]
        public async Task AddDishAsyncWrksCorrectly()
        {
            await this.PopulateDB();
            var file = this.GetFile("test");

            var addDish = new AddDishViewModel()
            {
                Name = "test",
                AdditionalInfo = "test",
                Weight = 50,
                Price = 10,
                Image = file,
                DishTypeId = 1,
                IngredientsId = new List<int>() { 1 },
                PrepareTime = 20,
            };

            var path = AppDomain.CurrentDomain.BaseDirectory;
            await this.DishService.AddDishAsync(addDish, path);

            Assert.Equal(1, this.DbContext.Dishes.Count());
        }

        [Fact]
        public async Task GetDishByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            var actual = this.DbContext.Dishes.FirstOrDefault(x => x.Id == dishId);
            var expected = this.DishService.GetDishById(dishId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public void GetDishByIdReturnsNullWhenIdIsNull()
        {
            var dish = this.DishService.GetDishById("invalid");

            Assert.Null(dish);
        }

        [Fact]
        public async Task GetDishWithDeletedByIdWorksCorrectlyWhenDeleted()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            var dish = this.DbContext.Dishes.FirstOrDefault(x => x.Id == dishId);
            dish.IsDeleted = true;
            dish.DeletedOn = DateTime.UtcNow;

            var expected = this.DishService.GetDishWithDeletedById(dishId);

            dish.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishWithDeletedByIdWorksCorrectlyWhenStillNotDeleted()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            var dish = this.DbContext.Dishes.FirstOrDefault(x => x.Id == dishId);

            var expected = this.DishService.GetDishWithDeletedById(dishId);

            dish.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetDishWithDeletedByIdReturnsNullWhenIdIsNotFound()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dish = this.DishService.GetDishWithDeletedById("Invalid");

            Assert.Null(dish);
        }

        [Fact]
        public async Task GetDishAsFoodItemByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            var dish = this.DbContext.Dishes.Where(x => x.Id == dishId).FirstOrDefault();

            var autoMapper = AutoMapperConfig.MapperInstance;
            var actual = autoMapper.Map<DishViewModel>(dish);
            var expected = this.DishService.GetDishAsFoodItemById(dishId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public void GetDishAsFoodItemReturnsNull()
        {
            var dish = this.DishService.GetDishAsFoodItemById("Invalid");

            Assert.Null(dish);
        }

        [Fact]
        public async Task GetAllDisheshWithDishTypeAsFoodItemWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishType = "test";
            var dish = this.DbContext.Dishes.Where(x => x.DishType.Name == dishType).ToList();

            var autoMapper = AutoMapperConfig.MapperInstance;
            var expected = autoMapper.Map<List<DishViewModel>>(dish);
            var actual = this.DishService.GetAllDisheshWithDishTypeAsFoodItem(dishType);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllDisheshWithDishTypeAsFoodItemReturns0Elements()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishType = "INVALID";
            var dish = this.DbContext.Dishes.Where(x => x.DishType.Name == dishType).ToList();

            var autoMapper = AutoMapperConfig.MapperInstance;
            var expected = autoMapper.Map<List<DishViewModel>>(dish);
            var actual = this.DishService.GetAllDisheshWithDishTypeAsFoodItem(dishType);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetEditDishViewModelByIdWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            var dish = this.DbContext.Dishes.FirstOrDefault(x => x.Id == dishId);

            var autoMapper = AutoMapperConfig.MapperInstance;
            var expected = autoMapper.Map<EditDishViewModel>(dish);
            var actual = this.DishService.GetEditDishViewModelById(dishId);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task GetEditDishViewModelByIdReturnsNullWhenIdIsInvalid()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var expected = this.DishService.GetEditDishViewModelById("Invalid");

            Assert.Null(expected);
        }

        [Fact]
        public async Task EditDishAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var newIngredient = new Ingredient()
            {
                Name = "test3",
                Id = 3,
            };

            this.DbContext.Ingredients.Add(newIngredient);
            await this.DbContext.SaveChangesAsync();
            var ingredient = this.DbContext.Ingredients.FirstOrDefault(x => x.Id == 1);
            var dishTypeId = 2;
            var ingredientId = 1;
            var image = this.GetFile("test123");
            var expectedName = "edit";
            var expectedIntValue = 33;
            var editDish = new EditDishViewModel()
            {
                Id = "test1",
                Name = expectedName,
                Price = expectedIntValue,
                Weight = expectedIntValue,
                PrepareTime = expectedIntValue,
                AdditionalInfo = expectedName,
                DishTypeId = dishTypeId,
                IngredientsId = new List<int>() { ingredientId, newIngredient.Id },
                NewImage = image,
            };

            await this.DishService.EditDishAsync(editDish, AppDomain.CurrentDomain.BaseDirectory);

            var actualIngredient = this.DbContext.Ingredients.FirstOrDefault(x => x.Id == ingredientId);
            var actual = this.DbContext.Dishes.FirstOrDefault(x => x.Id == "test1");

            Assert.Equal(editDish.Name, actual.Name);
            Assert.Equal(expectedIntValue, actual.Price);
            Assert.Equal(expectedIntValue, actual.Weight);
            Assert.Equal(expectedName, actual.AdditionalInfo);
            Assert.Equal(expectedIntValue, actual.PrepareTime);
            Assert.Equal(dishTypeId, actual.DishTypeId);
            Assert.Equal(new List<Ingredient>() { actualIngredient, newIngredient }, actual.Ingredients);
            Assert.Equal(ImageExtension.jpeg, actual.Image.Extension);
        }

        [Fact]
        public async Task EditDishAsyncThrowsExceptionWhenEditDishIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DishService.EditDishAsync(null, AppDomain.CurrentDomain.BaseDirectory));
        }

        [Fact]
        public async Task DeleteDishByIdAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            await this.AddDishesToDb();

            var dishId = "test1";
            await this.DishService.DeleteDishByIdAsync(dishId);

            var actual = this.DbContext.Dishes.FirstOrDefault(x => x.Id == dishId);

            Assert.Null(actual);
        }

        [Fact]
        public async Task DeleteDishByIdAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.DishService.DeleteDishByIdAsync("INVALID"));
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

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddDishesToDb()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test1", Extension = ImageExtension.jpeg},
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test2", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.SaveChangesAsync();
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
