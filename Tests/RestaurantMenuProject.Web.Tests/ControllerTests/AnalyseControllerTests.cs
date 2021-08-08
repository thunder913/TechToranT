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
    public class AnalyseControllerTests
    {
        [Fact]
        public void StaffShouldWorkCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Analyse/Staff")
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<AnalyseController>(c => c.Staff())
            .Which()
            .ShouldHave()
            .ActionAttributes(attr => attr
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(result => result
                .WithModelOfType<List<StaffAnalyseViewModel>>());
        }

        [Fact]
        public void SalesShouldWorkCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Analyse/Sales")
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<AnalyseController>(c => c.Sales())
            .Which()
            .ShouldHave()
            .ActionAttributes(attr => attr
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(result => result
                .WithModelOfType<SalesViewModel>());
        }
    }
}
