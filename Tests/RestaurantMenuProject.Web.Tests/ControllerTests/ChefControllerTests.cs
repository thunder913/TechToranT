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
    public class ChefControllerTests
    {
        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Chef/Index")
                .WithMethod(HttpMethod.Get)
                .WithUser(x => x.InRole(GlobalConstants.AdministratorRoleName)))
            .To<ChefController>(c => c.Index())
            .Which()
            .ShouldHave()
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .View(result => result
                .WithModelOfType<ChefViewModel>());
        }
    }
}
