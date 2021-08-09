using MyTested.AspNetCore.Mvc;
using RestaurantMenuProject.Web.Controllers;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Web.Tests.ControllerTests
{
    public class MenuControllerTests
    {
        [Theory]
        [InlineData("id", "DisplayFood")]
        [InlineData(null, "DisplayFoodType")]
        public void DisplayFoodWorksCorrectly(string id, string viewName)
        {
            var type = "Dish";

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/DisplayFood")
                .WithFormFields(new
                {
                    type = type,
                    id = id,
                })
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.DisplayFood(type, id))
            .Which()
            .ShouldHave()
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(viewName);
        }

        [Theory]
        [InlineData("id", "DisplayDrink")]
        [InlineData(null, "DisplayDrinkType")]
        public void DisplayDrinkWorksCorrectly(string id, string viewName)
        {
            var type = "Dish";

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/DisplayDrink")
                .WithFormFields(new
                {
                    type = type,
                    id = id,
                })
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.DisplayDrink(type, id))
            .Which()
            .ShouldHave()
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(viewName);
        }

        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/Index")
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.Index())
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.IsType<List<MenuItemViewModel>>(x.Model)));
        }

        [Fact]
        public void DrinksWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/Drinks")
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.Drinks())
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.IsType<List<MenuItemViewModel>>(x.Model)));
        }

        [Fact]
        public void SearchWorksCorrectlyWhenSearchTermIsNotNull()
        {
            var searchTerm = "a";
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/Search")
                .WithQuery("searchTerm", searchTerm)
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.Search(searchTerm))
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.IsType<SearchViewModel>(x.Model)));
        }

        [Fact]
        public void SearchWorksCorrectlyWhenSearchTermIsNull()
        {
            string searchTerm = null;

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Menu/Search")
                .WithQuery("searchTerm", searchTerm)
                .WithMethod(HttpMethod.Get))
            .To<MenuController>(c => c.Search(searchTerm))
            .Which()
            .ShouldReturn()
            .RedirectToAction("Index");
        }
    }
}
