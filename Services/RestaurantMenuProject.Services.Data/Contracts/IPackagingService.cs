namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Web.ViewModels;

    public interface IPackagingService
    {
        public ICollection<FoodTypeViewModel> GetAllPackagingTypes();
    }
}
