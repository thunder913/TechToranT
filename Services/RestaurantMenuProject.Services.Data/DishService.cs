namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class DishService : IDishService
    {
        private readonly IDeletableEntityRepository<Dish> dishRepository;

        public DishService(IDeletableEntityRepository<Dish> dishRepository)
        {
            this.dishRepository = dishRepository;
        }

        public async Task AddDish(Dish dish)
        {
            await this.dishRepository.AddAsync(dish);
            await this.dishRepository.SaveChangesAsync();
        }

        public void RemoveDish(Dish dish)
        {
            this.dishRepository.Delete(dish);
        }

        public Dish GetDishById(int id)
        {
            return this.dishRepository.AllAsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public FoodItemViewModel GetDishAsFoodItemById(int id)
        {
            return this.dishRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new FoodItemViewModel()
                    {
                        Id = x.Id,
                        DishType = x.DishType,
                        Name = x.Name,
                        PrepareTime = x.PrepareTime,
                        Price = x.Price,
                        Weight = x.Weight,
                        AdditionalInfo = x.AdditionalInfo,
                        Ingredients = x.Ingredients.Select(i => new IngredientViewModel()
                        {
                            Name = i.Name,
                            Allergens = i.Allergens.Select(a => new AllergenViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                            })
                            .ToList(),
                        }).ToList(),
                    })
                    .FirstOrDefault();
        }

        public ICollection<FoodItemViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.DishType.Name == dishType)
                        .Select(x => new FoodItemViewModel()
                        {
                            Id = x.Id,
                            DishType = x.DishType,
                            Name = x.Name,
                            PrepareTime = x.PrepareTime,
                            Price = x.Price,
                            Weight = x.Weight,
                        })
                        .ToList();
        }
    }
}
