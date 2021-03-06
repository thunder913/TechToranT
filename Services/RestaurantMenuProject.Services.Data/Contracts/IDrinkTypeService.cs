namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;
    using System.Collections.Generic;

    public interface IDrinkTypeService
    {
        public ICollection<DrinkType> GetAllDrinkTypes();
    }
}
