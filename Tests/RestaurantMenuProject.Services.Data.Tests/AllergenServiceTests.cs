namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using AutoMapper;
    using DeepEqual.Syntax;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using RestaurantMenuProject.Data;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Repositories;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class AllergenServiceTests : IDisposable
    {
        private readonly ApplicationDbContext dbContext;

        private readonly IDeletableEntityRepository<Allergen> allergenRepository;

        public AllergenServiceTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("db").Options;
            this.dbContext = new ApplicationDbContext(options);
            this.allergenRepository = new EfDeletableEntityRepository<Allergen>(this.dbContext);
        }

        [Fact]
        public async Task AddAllergenAsyncWorksCorrectly()
        {
            var allergenService = new AllergenService(this.allergenRepository);
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test" });

            Assert.True(this.allergenRepository.All().Count() == 1);

            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            Assert.True(this.allergenRepository.All().Count() == 2);

        }

        [Fact]
        public async Task AddAllergenAsyncThrowsExceptionIfNameAlreadyExists()
        {
            var allergenService = new AllergenService(this.allergenRepository);
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await Assert.ThrowsAsync<Exception>(() => allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" }));
        }

        [Fact]
        public async Task GetAllergensWithIdWorksCorrectly()
        {
            var allergenService = new AllergenService(this.allergenRepository);
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test1" });
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test2" });
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Name = "test3" });

            var actual = allergenService.GetAllergensWithId().ToList();
            var expected = this.allergenRepository.All().Select(x => new AllergenViewModel(){ Id = x.Id, Name = x.Name }).ToList();

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public async Task GetAllergensWithIdsShouldWorkCorrectly()
        {
            var allergenService = new AllergenService(this.allergenRepository);
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Id = 1, Name = "test1" });
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Id = 2, Name = "test2" });
            await allergenService.AddAllergenAsync(new AllergenViewModel() { Id = 3, Name = "test3" });

            var ids = new List<int>() { 1, 2 };
            var actual = allergenService.GetAllergensWithIds(ids);
            var expected = this.allergenRepository.All().Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);

            ids = new List<int>() { 3 };
            actual = allergenService.GetAllergensWithIds(ids);
            expected = this.allergenRepository.All().Where(x => ids.Contains(x.Id));

            expected.ShouldDeepEqual(actual);
        }

        public void Dispose()
        {
            this.dbContext.Database.EnsureDeleted();
        }
    }
}
