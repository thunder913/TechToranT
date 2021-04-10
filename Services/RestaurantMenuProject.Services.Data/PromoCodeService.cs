namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class PromoCodeService : IPromoCodeService
    {
        private readonly IDishTypeService dishTypeService;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IDeletableEntityRepository<PromoCode> promoCodeRepository;

        public PromoCodeService(
            IDishTypeService dishTypeService,
            IDrinkTypeService drinkTypeService,
            IDeletableEntityRepository<PromoCode> promoCodeRepository)
        {
            this.dishTypeService = dishTypeService;
            this.drinkTypeService = drinkTypeService;
            this.promoCodeRepository = promoCodeRepository;
        }

        public async Task AddPromoCodeAsync(AddPromoCodeViewModel addPromoCode)
        {
            var promoCode = new PromoCode()
            {
                ExpirationDate = addPromoCode.ExpirationDate,
                PromoPercent = addPromoCode.PromoPercent,
                MaxUsageTimes = addPromoCode.MaxUsageTimes,
                ValidDishCategories = this.dishTypeService.GetAllDishTypesWithIds(addPromoCode.ValidDishCategoriesId.ToArray()),
                ValidDrinkCategories = this.drinkTypeService.GetAllDrinkTypesWithIds(addPromoCode.ValidDrinkCategoriesId.ToArray()),
            };
            Random random = new Random();

            promoCode.Code = this.RandomString(random.Next(6, 10));

            await this.promoCodeRepository.AddAsync(promoCode);
            await this.promoCodeRepository.SaveChangesAsync();
        }

        private bool IsPromoCodeFree(string promoCode)
        {
            return this.promoCodeRepository.All().Any(x => x.Code == promoCode);
        }

        private string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomCode = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            if (this.IsPromoCodeFree(randomCode))
            {
                randomCode = this.RandomString(length);
            }

            return randomCode;
        }

        public ICollection<PromoCodeViewModel> GetAllPromoCodes(string sortColumn, string sortDirection, string searchValue)
        {
            var promoCodes = this.promoCodeRepository
                .AllAsNoTrackingWithDeleted()
                .To<PromoCodeViewModel>();

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                promoCodes = promoCodes.OrderBy(sortColumn + " " + sortDirection);
            }

            var dataToReturn = promoCodes.ToList();

            if (!string.IsNullOrEmpty(searchValue))
            {
                dataToReturn = dataToReturn.Where(m =>
                                            m.Code.ToString().Contains(searchValue)
                                            || m.ExpirationDate.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                            || m.MaxUsageTimes.ToString().Contains(searchValue)
                                            || m.UsedTimes.ToString().Contains(searchValue)
                                            || m.PromoPercent.ToString().Contains(searchValue))
                                            .ToList();
            }

            return dataToReturn;
        }

        public EditPromoCodeViewModel GetPromoCodeById(int id)
        {
            var promoCode = this.promoCodeRepository.AllWithDeleted().Include(x => x.ValidDrinkCategories).Include(x => x.ValidDishCategories).FirstOrDefault(x => x.Id == id);

            if (promoCode == null)
            {
                throw new InvalidOperationException("There is no promo code with this id!");
            }

            var toReturn = new EditPromoCodeViewModel()
            {
                ExpirationDate = promoCode.ExpirationDate,
                MaxUsageTimes = promoCode.MaxUsageTimes,
                PromoPercent = promoCode.PromoPercent,
                ValidDishCategoriesId = promoCode.ValidDishCategories.Select(x => x.Id).ToList(),
                ValidDrinkCategoriesId = promoCode.ValidDrinkCategories.Select(x => x.Id).ToList(),
                Code = promoCode.Code,
            };

            return toReturn;
        }

        public async Task EditPromoCodeAsync(EditPromoCodeViewModel editViewModel)
        {
            var promoCode = this.promoCodeRepository.AllWithDeleted().Include(x => x.ValidDrinkCategories).Include(x => x.ValidDishCategories).FirstOrDefault(x => x.Id == editViewModel.Id);

            if (promoCode == null)
            {
                throw new InvalidOperationException("There is no promo code with this id!");
            }

            // Removing the items that are not containted in the new promo code
            foreach (var dish in promoCode.ValidDishCategories)
            {
                if (!editViewModel.ValidDishCategoriesId.Contains(dish.Id))
                {
                    promoCode.ValidDishCategories.Remove(dish);
                }
            }

            foreach (var drink in promoCode.ValidDrinkCategories)
            {
                if (!editViewModel.ValidDrinkCategoriesId.Contains(drink.Id))
                {
                    promoCode.ValidDrinkCategories.Remove(drink);
                }
            }

            // Adding the items that are not containted in the old promo code

            foreach (var id in editViewModel.ValidDishCategoriesId)
            {
                if (!promoCode.ValidDishCategories.Any(x => x.Id == id))
                {
                    var dishType = this.dishTypeService.GetDishTypeById(id);
                    promoCode.ValidDishCategories.Add(dishType);
                }
            }

            foreach (var id in editViewModel.ValidDrinkCategoriesId)
            {
                if (!promoCode.ValidDrinkCategories.Any(x => x.Id == id))
                {
                    var drinkType = this.drinkTypeService.GetDrinkTypeById(id);
                    promoCode.ValidDrinkCategories.Add(drinkType);
                }
            }

            promoCode.ExpirationDate = editViewModel.ExpirationDate;
            promoCode.MaxUsageTimes = editViewModel.MaxUsageTimes;
            promoCode.PromoPercent = editViewModel.PromoPercent;

            await this.promoCodeRepository.SaveChangesAsync();
            }

        public async Task<PromoCode> GetPromoCodeByCodeAsync(string code)
        {
            var promoCode = this.promoCodeRepository.All().FirstOrDefault(x => x.Code == code);

            if (promoCode == null)
            {
                throw new InvalidOperationException("There is no promo code with the given code!");
            }
            else if (promoCode.ExpirationDate <= DateTime.UtcNow)
            {
                this.promoCodeRepository.Delete(promoCode);
                await this.promoCodeRepository.SaveChangesAsync();
                throw new Exception("The promo code has expired!");
            }
            else if (promoCode.UsedTimes >= promoCode.MaxUsageTimes)
            {
                this.promoCodeRepository.Delete(promoCode);
                await this.promoCodeRepository.SaveChangesAsync();
                throw new Exception("This is no longer valid!");
            }

            return promoCode;
        }

        public async Task UsePromoCodeAsync(int id, int count)
        {
            var promoCode = this.promoCodeRepository.All().FirstOrDefault(x => x.Id == id);

            if (promoCode == null)
            {
                throw new Exception("There is no promo code with this id!");
            }

            if (promoCode.ExpirationDate <= DateTime.UtcNow || promoCode.UsedTimes >= promoCode.MaxUsageTimes)
            {
                throw new Exception("The promo code is no longer valid!");
            }

            promoCode.UsedTimes += count;

            await this.promoCodeRepository.SaveChangesAsync();
        }
    }
}
