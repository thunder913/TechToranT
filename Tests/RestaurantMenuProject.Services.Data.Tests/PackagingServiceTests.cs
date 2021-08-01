using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class PackagingServiceTests : BaseServiceTests
    {
        private IPackagingService PackagingService => this.ServiceProvider.GetRequiredService<IPackagingService>();

        [Fact]
        public async Task GetAllPackagingTypesWorksCorrectly()
        {
            await this.PopulateDB();

            var actual = this.PackagingService.GetAllPackagingTypes();
            var expected = this.DbContext.PackagingTypes.Select(x => new FoodTypeViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            }).ToList();

            actual.ShouldDeepEqual(expected);
        }

        private async Task PopulateDB()
        {
            var packagingType1 = new PackagingType()
            {
                Id = 1,
                Name = "test1",
                IsEco = true,
            };

            var packagingType2 = new PackagingType()
            {
                Id = 2,
                Name = "test2",
                IsEco = true,
            };

            var packagingType3 = new PackagingType()
            {
                Id = 3,
                Name = "test3",
                IsEco = true,
            };

            await this.DbContext.PackagingTypes.AddAsync(packagingType1);
            await this.DbContext.PackagingTypes.AddAsync(packagingType2);
            await this.DbContext.PackagingTypes.AddAsync(packagingType3);

            await this.DbContext.SaveChangesAsync();
        }
    }
}
