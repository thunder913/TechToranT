namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkTypeService : IDrinkTypeService
    {
        private readonly IDeletableEntityRepository<DrinkType> drinkRepository;

        public DrinkTypeService(IDeletableEntityRepository<DrinkType> drinkRepository)
        {
            this.drinkRepository = drinkRepository;
        }

        public ICollection<MenuItemViewModel> GetAllDrinkTypes()
        {
            return this.drinkRepository.AllAsNoTracking().Select(x=> new MenuItemViewModel()
            {
                Description = x.Description,
                Name = x.Name,
            }).ToList();
        }
    }
}
