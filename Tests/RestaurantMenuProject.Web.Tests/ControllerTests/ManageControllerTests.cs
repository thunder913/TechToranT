using MyTested.AspNetCore.Mvc;
using RestaurantMenuProject.Common;
using RestaurantMenuProject.Web.Controllers;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
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
                .WithUser())
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
            .View(result => result.Passing(x => Assert.Equal(expectedViewType, x.Model.GetType().Name)));
        }
    }
}
