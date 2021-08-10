namespace RestaurantMenuProject.Web.Tests.ControllerTests.Api
{
    using System;

    using MyTested.AspNetCore.Mvc;
    using RestaurantMenuProject.Web.Controllers.Api;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class AnalyzeControllerTests
    {
        [Fact]
        public void GetIncomesWorksCorrectly()
        {
            var incomesViewModel = new GetIncomesViewModel()
            {
                StartDate = DateTime.Now.AddDays(-5).ToShortDateString(),
                EndDate = DateTime.Now.ToShortDateString(),
                Period = "daily",
            };

            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithLocation("/api/Analyse/Incomes")
                .WithQuery(incomesViewModel)
                .WithMethod(HttpMethod.Get)
                .WithUser())
            .To<AnalyseController>(c => c.GetIncomes(incomesViewModel))
            .Which()
            .ShouldHave()
            .ValidModelState()
            .AndAlso()
            .ShouldReturn()
            .ResultOfType<SalesViewModel>();
        }
    }
}
