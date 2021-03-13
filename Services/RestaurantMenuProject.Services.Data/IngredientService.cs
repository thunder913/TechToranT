using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class IngredientService : IIngredientService
    {
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;
        private readonly IAllergenService allergenService;

        public IngredientService(
            IDeletableEntityRepository<Ingredient> ingredientRepository,
            IAllergenService allergenService
            )
        {
            this.ingredientRepository = ingredientRepository;
            this.allergenService = allergenService;
        }

        public async Task AddIngredient(AddIngredientViewModel ingredient)
        {
            var ingredientToAdd = new Ingredient()
            {
                Name = ingredient.Name,
                Allergens = this.allergenService.GetAllergensWithIds(ingredient.AllergensId.ToList()).ToList(),
            };
            await this.ingredientRepository.AddAsync(ingredientToAdd);
            await this.ingredientRepository.SaveChangesAsync();
        }

        public ICollection<DishIngredientViewModel> GetAllAsDishIngredientViewModel()
        {
            return this.ingredientRepository
                .AllAsNoTracking()
                .To<DishIngredientViewModel>()
                .ToList();
        }

        public ICollection<Ingredient> GetAllIngredientsByIds(int[] ids)
        {
            return this.ingredientRepository
                .All()
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }
    }
}
