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
    public class OrderControllerTests
    {
        [Fact]
        public void AllWorksCorrectly()
        {
            string userId = "userId";

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Order/All/" + userId + "/1")
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<OrderController>(c => c.All(userId, 1))
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.IsType<OrderViewModel>(x.Model)));
        }

        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Order/Index/id")
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<OrderController>(c => c.Index("id"))
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.True(x.Model is OrderInfoViewModel || x.Model == null)));
        }
    }
}
