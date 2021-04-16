using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class PromoCodeServiceTests : BaseServiceTests
    {
        private IPromoCodeService PromoCodeService => this.ServiceProvider.GetRequiredService<IPromoCodeService>();

        [Fact]
        public async Task AddPromoCodeAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var dishTypes = this.DbContext.DishTypes.ToList();
            var drinkType = this.DbContext.DrinkTypes.ToList();

            var addPromoCode = new AddPromoCodeViewModel()
            {
                ExpirationDate = DateTime.Now.AddDays(1),
                PromoPercent = 99,
                MaxUsageTimes = 99,
                ValidDishCategoriesId = new List<int>() { 1, 2, },
                ValidDrinkCategoriesId = new List<int>() { 3 },
            };

            await this.PromoCodeService.AddPromoCodeAsync(addPromoCode);

            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.PromoPercent == 99 && x.MaxUsageTimes == 99);

            Assert.Equal(99, promoCode.PromoPercent);
            Assert.Equal(99, promoCode.MaxUsageTimes);
            Assert.Equal(2, promoCode.ValidDishCategories.Count);
            Assert.Equal(1, promoCode.ValidDrinkCategories.Count);
        }

        private async Task PopulateDB()
        {
            await this.DbContext.DrinkTypes.AddAsync(new DrinkType()
            {
                Id = 3,
                Name = "test3",
            });

            await this.DbContext.DishTypes.AddAsync(new DishType()
            {
                Id = 1,
                Name = "test1",
            });

            await this.DbContext.DishTypes.AddAsync(new DishType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();

            var dishTypes = this.DbContext.DishTypes.ToList();
            var drinkTypes = this.DbContext.DrinkTypes.ToList();

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 1,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code1",
                PromoPercent = 20,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 2,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code2",
                PromoPercent = 10,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.SaveChangesAsync();
        }
    }
}
