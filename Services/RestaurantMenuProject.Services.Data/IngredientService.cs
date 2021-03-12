using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class IngredientService : IIngredientService
    {
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;

        public IngredientService(IDeletableEntityRepository<Ingredient> ingredientRepository)
        {
            this.ingredientRepository = ingredientRepository;
        }

        public async Task AddIngredient(Ingredient ingredient)
        {
            await this.ingredientRepository.AddAsync(ingredient);
            await this.ingredientRepository.SaveChangesAsync();
        }

        public ICollection<DishIngredientViewModel> GetAllAsDishIngredientViewModel()
        {
            return this.ingredientRepository.AllAsNoTracking().Select(x => new DishIngredientViewModel()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToList();
        }

        public ICollection<Ingredient> GetAllIngredientsByIds(int[] ids)
        {
            return this.ingredientRepository.All().Where(x => ids.Contains(x.Id)).ToList();
        }
    }
}
