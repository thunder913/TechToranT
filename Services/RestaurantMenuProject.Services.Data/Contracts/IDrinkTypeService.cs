namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public interface IDrinkTypeService
    {
        public ICollection<MenuItemViewModel> GetAllDrinkTypes();
    }
}
