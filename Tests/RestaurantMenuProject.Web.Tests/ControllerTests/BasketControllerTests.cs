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
    public class BasketControllerTests
    {
        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Basket/Index")
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<BasketController>(c => c.Index())
            .Which()
            .ShouldHave()
            .ActionAttributes(attr => attr
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(result => result
                .WithModelOfType<BasketViewModel>()
                .AndAlso());
        }
    }
}
