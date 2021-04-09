namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public ICollection<ManageOrderViewModel> GetAllPromoCodes(string sortColumn, string sortDirection, string searchValue)
        {
            var promoCode = this.promoCodeRepository
                .AllAsNoTrackingWithDeleted()
                .AsQueryable();

            // TODO Fix this code

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                promoCode = promoCode.OrderBy(sortColumn + " " + sortDirection);
            }

            var dataToReturn = promoCode.To<ManageOrderViewModel>().ToList();

            if (!string.IsNullOrEmpty(searchValue))
            {
                dataToReturn = dataToReturn.Where(m =>
                                            m.Price.ToString().Contains(searchValue)
                                            || m.Email.ToLower().Contains(searchValue.ToLower())
                                            || m.Status.ToString().ToLower().Contains(searchValue.ToLower())
                                            || m.Date.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                            || m.FullName.ToLower().Contains(searchValue)).ToList(); // TODO fix it again to make it do it all as Queryable
            }

            return dataToReturn;
        }
    }
}
