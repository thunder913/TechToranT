using MyTested.AspNetCore.Mvc;
using RestaurantMenuProject.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Web.Tests.ControllerTests
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexShouldReturnHomePage()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/")
                .To<HomeController>(c => c.Index())
                .Which()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void PrivacyShouldReturnPrivacyPage()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Home/Privacy")
                .To<HomeController>(c => c.Privacy())
                .Which()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void ErrorShouldReturnErrorPage()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Home/Error")
                .To<HomeController>(c => c.Error())
                .Which()
                .ShouldReturn()
                .View();
        }
    }
}
