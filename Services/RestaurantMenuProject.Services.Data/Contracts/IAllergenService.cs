namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IAllergenService
    {
        public ICollection<AllergenViewModel> GetAllergensWithId();

        public Task AddAllergenAsync(AllergenViewModel allergen);

        public ICollection<Allergen> GetAllergensWithIds(List<int> ids);
    }
}
