using DeepEqual.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
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

        [Fact]
        public async Task GetAllPromoCodesWorksCorrectlyWithoutOrdering()
        {
            await this.PopulateDB();

            var actual = this.PromoCodeService.GetAllPromoCodes("", "", "");
            var expected = this.DbContext.PromoCodes.To<PromoCodeViewModel>();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetAllPromoCodesWorksCorrectlyWithoutSearchValue()
        {
            await this.PopulateDB();

            var actual = this.PromoCodeService.GetAllPromoCodes("code", "desc", string.Empty);
            var expected = this.DbContext.PromoCodes.OrderByDescending(x => x.Code).To<PromoCodeViewModel>().ToList();

            actual.ShouldDeepEqual(expected);

        }

        [Theory]
        [InlineData("c")]
        [InlineData("20")]
        [InlineData("10")]
        [InlineData(":")]
        [InlineData("/")]
        [InlineData("code1")]
        [InlineData("0")]
        public async Task GetAllPromoCodesWorksCorrectlyWithSearchValue(string searchValue)
        {
            await this.PopulateDB();
            var expected = this.DbContext.PromoCodes.OrderByDescending(x => x.Id).Where(m =>
                            m.Code.ToString().Contains(searchValue)
                            || m.ExpirationDate.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                            || m.MaxUsageTimes.ToString().Contains(searchValue)
                            || m.UsedTimes.ToString().Contains(searchValue)
                            || m.PromoPercent.ToString().Contains(searchValue))
                            .To<PromoCodeViewModel>()
                            .ToList();

            var actual = this.PromoCodeService.GetAllPromoCodes("id", "desc", searchValue);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetPromoCodeByIdWorksCorrectly()
        {
            await this.PopulateDB();

            var id = 1;
            var promoCode = this.DbContext.PromoCodes
                .Include(x => x.ValidDishCategories)
                .Include(x => x.ValidDrinkCategories)
                .FirstOrDefault(x => x.Id == id);

            var actual = this.PromoCodeService.GetPromoCodeById(id);
            var expected = new EditPromoCodeViewModel()
            {
                ExpirationDate = promoCode.ExpirationDate,
                MaxUsageTimes = promoCode.MaxUsageTimes,
                PromoPercent = promoCode.PromoPercent,
                ValidDishCategoriesId = promoCode.ValidDishCategories.Select(y => y.Id).ToList(),
                ValidDrinkCategoriesId = promoCode.ValidDrinkCategories.Select(y => y.Id).ToList(),
                Code = promoCode.Code,
            };

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetPromoCodeByIdThrosWhenGivenInvalidId()
        {
            Assert.Throws<InvalidOperationException>(() => this.PromoCodeService.GetPromoCodeById(999));
        }

        [Fact]
        public async Task GetPromoCodeByCodeAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var code = "code1";
            var expected = this.DbContext.PromoCodes.FirstOrDefault(x => x.Code == code);
            var actual =  await this.PromoCodeService.GetPromoCodeByCodeAsync(code);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task GetPromoCodeByCodeAsyncThrowsExceptionWhenTheCodeIsInvalid()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.PromoCodeService.GetPromoCodeByCodeAsync("invalid!"));
        }

        [Fact]
        public async Task GetPromoCodeByCodeAsyncThrowsExceptionWhenTheCodeHasExpired()
        {
            await this.PopulateDB();
            var code = "code1";

            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Code == code);
            promoCode.ExpirationDate = DateTime.UtcNow.AddDays(-1);
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.PromoCodeService.GetPromoCodeByCodeAsync(code));
        }

        [Fact]
        public async Task GetPromoCodeByCodeAsyncThrowsExceptionWhenOverused()
        {
            await this.PopulateDB();
            var code = "code1";

            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Code == code);
            promoCode.MaxUsageTimes = 1;
            promoCode.UsedTimes = 5;
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.PromoCodeService.GetPromoCodeByCodeAsync(code));
        }

        [Fact]
        public async Task UsePromoCodeAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var codeId = 1;
            var useTimes = 5;
            var expectedUsed = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId).UsedTimes+useTimes;
            await this.PromoCodeService.UsePromoCodeAsync(codeId, useTimes);
            var actualUsed = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId).UsedTimes;

            Assert.Equal(expectedUsed, actualUsed);
        }

        [Fact]
        public async Task UsePromoCodeAsyncThrowsExceptionWhenIdIsInvalid()
        {
            await Assert.ThrowsAsync<Exception>(async () => await this.PromoCodeService.UsePromoCodeAsync(99992, 5));
        }

        [Fact]
        public async Task UsePromoCodeAsyncThrowsExceptionWhenCodeHasExpired()
        {
            await this.PopulateDB();
            var id = 1;

            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == id);
            promoCode.ExpirationDate = DateTime.UtcNow.AddDays(-1);
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.PromoCodeService.UsePromoCodeAsync(id, 5));
        }

        [Fact]
        public async Task GetPromoCodeByCodeAsyncThrowsExceptionWhenCodeIsOverused()
        {
            await this.PopulateDB();
            var id = 1;

            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == id);
            promoCode.MaxUsageTimes = 1;
            promoCode.UsedTimes = 5;
            await this.DbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<Exception>(async () => await this.PromoCodeService.UsePromoCodeAsync(id, 5));
        }

        [Fact]
        public async Task EditPromoCodeAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            await this.DbContext.DrinkTypes.AddAsync(new DrinkType()
            {
                Id = 5,
                Name = "new",
            });

            await this.DbContext.DishTypes.AddAsync(new DishType()
            {
                Id = 5,
                Name = "new",
            });
            await this.DbContext.SaveChangesAsync();

            var codeId = 1;
            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);
            var editPromoCodeViewModel = new EditPromoCodeViewModel()
            {
                Code = "code1",
                ExpirationDate = promoCode.ExpirationDate.AddDays(1),
                Id = codeId,
                MaxUsageTimes = promoCode.MaxUsageTimes + 5,
                PromoPercent = promoCode.PromoPercent + 5,
                ValidDishCategoriesId = new List<int>() { 5, },
                ValidDrinkCategoriesId = new List<int>() { 5, },
            };
            await this.PromoCodeService.EditPromoCodeAsync(editPromoCodeViewModel);

            var actual = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);

            Assert.Equal(editPromoCodeViewModel.Code, actual.Code);
            Assert.Equal(editPromoCodeViewModel.ExpirationDate, actual.ExpirationDate);
            Assert.Equal(editPromoCodeViewModel.MaxUsageTimes, actual.MaxUsageTimes);
            Assert.Equal(editPromoCodeViewModel.PromoPercent, actual.PromoPercent);
            Assert.Equal(1, actual.ValidDishCategories.Count);
            Assert.Equal(1, actual.ValidDishCategories.Count);
        }

        [Fact]
        public async Task EditPromoCodeAsyncWorksCorrectlyWithManyDishAndDrinkTypes()
        {
            await this.PopulateDB();

            await this.DbContext.DrinkTypes.AddAsync(new DrinkType()
            {
                Id = 5,
                Name = "new",
            });

            await this.DbContext.DishTypes.AddAsync(new DishType()
            {
                Id = 5,
                Name = "new",
            });
            await this.DbContext.SaveChangesAsync();

            var codeId = 1;
            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);
            var editPromoCodeViewModel = new EditPromoCodeViewModel()
            {
                Code = "code1",
                ExpirationDate = promoCode.ExpirationDate.AddDays(1),
                Id = codeId,
                MaxUsageTimes = promoCode.MaxUsageTimes + 5,
                PromoPercent = promoCode.PromoPercent + 5,
                ValidDishCategoriesId = new List<int>() { 5, 1, },
                ValidDrinkCategoriesId = new List<int>() { 5, 3, },
            };
            await this.PromoCodeService.EditPromoCodeAsync(editPromoCodeViewModel);

            var actual = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);

            Assert.Equal(editPromoCodeViewModel.Code, actual.Code);
            Assert.Equal(editPromoCodeViewModel.ExpirationDate, actual.ExpirationDate);
            Assert.Equal(editPromoCodeViewModel.MaxUsageTimes, actual.MaxUsageTimes);
            Assert.Equal(editPromoCodeViewModel.PromoPercent, actual.PromoPercent);
            Assert.Equal(2, actual.ValidDishCategories.Count);
            Assert.Equal(2, actual.ValidDishCategories.Count);
        }


        [Fact]
        public async Task EditPromoCodeAsyncWorksCorrectlyWhenEditingWithoutCategories()
        {
            await this.PopulateDB();

            var codeId = 1;
            var promoCode = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);
            var editPromoCodeViewModel = new EditPromoCodeViewModel()
            {
                Code = "code1",
                ExpirationDate = promoCode.ExpirationDate.AddDays(1),
                Id = codeId,
                MaxUsageTimes = promoCode.MaxUsageTimes + 5,
                PromoPercent = promoCode.PromoPercent + 5,
                ValidDishCategoriesId = new List<int>(),
                ValidDrinkCategoriesId = new List<int>(),
            };
            await this.PromoCodeService.EditPromoCodeAsync(editPromoCodeViewModel);

            var actual = this.DbContext.PromoCodes.FirstOrDefault(x => x.Id == codeId);

            Assert.Equal(editPromoCodeViewModel.Code, actual.Code);
            Assert.Equal(editPromoCodeViewModel.ExpirationDate, actual.ExpirationDate);
            Assert.Equal(editPromoCodeViewModel.MaxUsageTimes, actual.MaxUsageTimes);
            Assert.Equal(editPromoCodeViewModel.PromoPercent, actual.PromoPercent);
            Assert.Equal(0, actual.ValidDishCategories.Count);
            Assert.Equal(0, actual.ValidDishCategories.Count);
        }

        [Fact]
        public async Task EditPromoCodeAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.PromoCodeService.EditPromoCodeAsync(new EditPromoCodeViewModel() { Id = 99123 }));
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
