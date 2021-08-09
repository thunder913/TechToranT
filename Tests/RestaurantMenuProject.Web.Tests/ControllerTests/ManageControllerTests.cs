using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using MyTested.AspNetCore.Mvc;
using RestaurantMenuProject.Common;
using RestaurantMenuProject.Data;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Web.Controllers;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Web.Tests.ControllerTests
{
    public class ManageControllerTests
    {
        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Manage/Index")
                .WithMethod(HttpMethod.Get)
                .WithUser(u => u.InRole(GlobalConstants.AdministratorRoleName)))
            .To<ManageController>(c => c.Index())
            .Which()
            .ShouldReturn()
            .View();
        }

        [Theory]
        [InlineData("Dish", "AddDishViewModel")]
        [InlineData("Drink", "AddDrinkViewModel")]
        [InlineData("Ingredient", "AddIngredientViewModel")]
        [InlineData("Allergen", "AllergenViewModel")]
        [InlineData("DishCategory", "AddCategoryViewModel")]
        [InlineData("DrinkCategory", "AddCategoryViewModel")]
        [InlineData("Invalid", null)]
        public void AddShouldWorkCorrectly(string type, string expectedViewType)
        {

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Manage/Add")
                .WithFormFields(new
                {
                    type = type,
                })
                .WithMethod(HttpMethod.Post)
                .WithUser(u => u.InRole(GlobalConstants.AdministratorRoleName))
                .WithAntiForgeryToken())
            .To<ManageController>(c => c.Add(type))
            .Which()
            .ShouldHave()
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(result => result.Passing(x => Assert.True(expectedViewType == null || expectedViewType == x.Model.GetType().Name)));
        }

        [Fact]
        public void AddDrinkShouldWorkCorrectly()
        {
            var addDrinkModel = new AddDrinkViewModel()
            {
                Name = "test",
                Price = 49.02m,
                Weight = 250,
                AlchoholByVolume = 10,
                AdditionalInfo = "This is additional info",
                DrinkTypeId = 3,
                IngredientsId = new List<int>() { 1, 2 },
                PackagingTypeId = 1,
                Image = this.GetImageMock(),
            };

            MyController<ManageController>
                .Calling(c => c.AddDrink(addDrinkModel))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addDrinkModel.Weight = -5;

            MyController<ManageController>
            .Calling(c => c.AddDrink(addDrinkModel))
            .ShouldReturn()
            .View();

            //MyMvc
            //.Pipeline()
            //.ShouldMap(request => request
            //    .WithContentType("multipart/form-data")
            //    .WithLocation("/Manage/AddDish")
            //    .WithFormFields(addDishModel)
            //    .WithMethod(HttpMethod.Post)
            //    .WithUser(u => u.InRole(GlobalConstants.AdministratorRoleName))
            //    .WithAntiForgeryToken())
            //.To<ManageController>(c => c.AddDish(addDishModel))
            //.Which()
            //.ShouldHave()
            //.ValidModelState()
            //.AndAlso()
            //.ShouldReturn()
            //.View();
        }

        [Fact]
        public void AddDishShouldWorkCorrectly()
        {
            var addDishModel = new AddDishViewModel()
            {
                Name = "test",
                Price = 49.02m,
                Weight = 250,
                PrepareTime = 10,
                AdditionalInfo = "This is additional info",
                DishTypeId = 3,
                IngredientsId = new List<int>() { 1, 2 },
                Image = this.GetImageMock(),
            };

            MyController<ManageController>
                .Calling(c => c.AddDish(addDishModel))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addDishModel.Weight = -5;

            MyController<ManageController>
            .Calling(c => c.AddDish(addDishModel))
            .ShouldReturn()
            .View();
        }

        [Fact]
        public void AddIngredientShouldWorkCorrectly()
        {
            var addIngredient = new AddIngredientViewModel()
            {
                Name = "test",
                AllergensId = new List<int>() { 1, 2 },
            };

            MyController<ManageController>
                .Calling(c => c.AddIngredient(addIngredient))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addIngredient.Name = null;

            MyController<ManageController>
            .Calling(c => c.AddIngredient(addIngredient))
            .ShouldReturn()
            .View();
        }

        [Fact]
        public void AddAllergenShouldWorkCorrectly()
        {
            var addAllergen = new AllergenViewModel()
            {
                Name = "test",
            };

            MyController<ManageController>
                .Calling(c => c.AddAllergen(addAllergen))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addAllergen.Name = null;

            MyController<ManageController>
            .Calling(c => c.AddAllergen(addAllergen))
            .ShouldReturn()
            .View();
        }


        [Fact]
        public void AddDishCategoryWorksCorrectly()
        {
            var addCategoryViewModel = new AddCategoryViewModel()
            {
                Name = "Test",
                Description = "testdescritption",
                Image = this.GetImageMock(),
            };


            MyController<ManageController>
                .Calling(c => c.AddDishCategory(addCategoryViewModel))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addCategoryViewModel.Name = null;

            MyController<ManageController>
            .Calling(c => c.AddDishCategory(addCategoryViewModel))
            .ShouldReturn()
            .View();
        }

        [Fact]
        public void AddDrinkCategoryWorksCorrectly()
        {
            var addCategoryViewModel = new AddCategoryViewModel()
            {
                Name = "Test",
                Description = "testdescritption",
                Image = this.GetImageMock(),
            };


            MyController<ManageController>
                .Calling(c => c.AddDrinkCategory(addCategoryViewModel))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index");

            addCategoryViewModel.Name = null;

            MyController<ManageController>
            .Calling(c => c.AddDrinkCategory(addCategoryViewModel))
            .ShouldReturn()
            .View();
        }

        private IFormFile GetImageMock()
        {
            var imageMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            imageMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            imageMock.Setup(_ => _.FileName).Returns(fileName);
            imageMock.Setup(_ => _.Length).Returns(ms.Length);

            return imageMock.Object;
        }
    }
}
