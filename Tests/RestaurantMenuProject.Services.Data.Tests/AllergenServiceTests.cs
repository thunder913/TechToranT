namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DeepEqual.Syntax;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class AllergenServiceTests : BaseServiceTests
    {

        private IAllergenService AllergenService => this.ServiceProvider.GetRequiredService<IAllergenService>();

        [Fact]
        public async Task AddAllergenAsyncWorksCorrectly()
        {
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test" });

            Assert.True(this.DbContext.Allergens.Count() == 1);

            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            Assert.True(this.DbContext.Allergens.Count() == 2);

        }

        [Fact]
        public async Task AddAllergenAsyncThrowsExceptionIfNameAlreadyExists()
        {
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await Assert.ThrowsAsync<Exception>(() => this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" }));
        }

        [Fact]
        public async Task GetAllergensWithIdWorksCorrectly()
        {
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test1" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test3" });

            var actual = this.AllergenService.GetAllergensWithId().ToList();
            var expected = this.DbContext.Allergens.Select(x => new AllergenViewModel(){ Id = x.Id, Name = x.Name }).ToList();

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public async Task GetAllergensWithIdsShouldWorkCorrectly()
        {
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 1, Name = "test1" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 2, Name = "test2" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 3, Name = "test3" });

            var ids = new List<int>() { 1, 2 };
            var actual = this.AllergenService.GetAllergensWithIds(ids);
            var expected = this.DbContext.Allergens.Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);

            ids = new List<int>() { 3 };
            actual = this.AllergenService.GetAllergensWithIds(ids);
            expected = this.DbContext.Allergens.Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);
        }
    }
}
