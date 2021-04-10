namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IPromoCodeService
    {
        public Task AddPromoCodeAsync(AddPromoCodeViewModel addPromoCode);

        public ICollection<PromoCodeViewModel> GetAllPromoCodes(string sortColumn, string sortDirection, string searchValue);

        public EditPromoCodeViewModel GetPromoCodeById(int id);

        public Task EditPromoCodeAsync(EditPromoCodeViewModel editViewModel);

        public Task<PromoCode> GetPromoCodeByCodeAsync(string code);

        public Task UsePromoCodeAsync(int id, int count);
    }
}
