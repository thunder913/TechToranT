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
    public class WaiterControllerTests
    {
        [Fact]
        public void IndexWorksCorrectly()
        {
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/Waiter/Index")
                .WithMethod(HttpMethod.Get)
                .WithUser(x => x.InRole(GlobalConstants.AdministratorRoleName)))
            .To<WaiterController>(c => c.Index())
            .Which()
            .ShouldReturn()
            .View(v => v.Passing(x => Assert.IsType<WaiterViewModel>(x.Model)));
        }
    }
}
