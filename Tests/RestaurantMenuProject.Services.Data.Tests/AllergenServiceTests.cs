namespace RestaurantMenuProject.Services.Data.Tests
{
    using AutoMapper;
    using DeepEqual.Syntax;
    using Moq;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AllergenServiceTests
    {
        public AllergenServiceTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
        }

        private AllergenService AllergenService { get; set; }

        private List<Allergen> Allergens { get; set; }

        [Fact]
        public async Task AddAllergenAsyncWorksCorrectly()
        {
            this.SetupMoq();
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test" });

            Assert.True(this.Allergens.Count == 1);

            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            Assert.True(this.Allergens.Count == 2);

        }

        [Fact]
        public async Task AddAllergenAsyncThrowsExceptionIfNameAlreadyExists()
        {
            this.SetupMoq();

            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await Assert.ThrowsAsync<Exception>(() => this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" }));
        }

        [Fact]
        public async Task GetAllergensWithIdWorksCorrectly()
        {
            this.SetupMoq();
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test1" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test3" });

            var actual = this.AllergenService.GetAllergensWithId().ToList();
            var expected = this.Allergens.Select(x => new AllergenViewModel(){ Id = x.Id, Name = x.Name }).ToList();

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public async Task GetAllergensWithIdsShouldWorkCorrectly()
        {
            this.SetupMoq();

            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 1, Name = "test1" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 2, Name = "test2" });
            await this.AllergenService.AddAllergenAsync(new AllergenViewModel() { Id = 3, Name = "test3" });

            var ids = new List<int>() { 1, 2 };
            var actual = this.AllergenService.GetAllergensWithIds(ids);
            var expected = this.Allergens.Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);

            ids = new List<int>() { 3 };
            actual = this.AllergenService.GetAllergensWithIds(ids);
            expected = this.Allergens.Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);
        }

        private void SetupMoq()
        {
            this.Allergens = new List<Allergen>();
            var allergenRepo = new Mock<IDeletableEntityRepository<Allergen>>();
            allergenRepo.Setup(x => x.AddAsync(It.IsAny<Allergen>())).Callback((Allergen all) => this.Allergens.Add(all));
            allergenRepo.Setup(x => x.All()).Returns(this.Allergens.AsQueryable());
            allergenRepo.Setup(x => x.AllAsNoTracking()).Returns(this.Allergens.AsQueryable);
            this.AllergenService = new AllergenService(allergenRepo.Object);
        }

    }
}
