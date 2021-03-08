namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Web.ViewModels;
    using System.Collections.Generic;

    public interface IAllergenService
    {
        public ICollection<AllergenViewModel> GetAllergensWithId();
    }
}
