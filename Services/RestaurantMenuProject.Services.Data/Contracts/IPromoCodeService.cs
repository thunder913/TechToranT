namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using RestaurantMenuProject.Web.ViewModels;

    public interface IPromoCodeService
    {
        public Task AddPromoCodeAsync(AddPromoCodeViewModel addPromoCode);
    }
}
