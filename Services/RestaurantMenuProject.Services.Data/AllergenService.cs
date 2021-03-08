using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Services.Data
{
    public class AllergenService : IAllergenService
    {
        private readonly IDeletableEntityRepository<Allergen> allergenRepository;

        public AllergenService(IDeletableEntityRepository<Allergen> allergenRepository)
        {
            this.allergenRepository = allergenRepository;
        }
        public ICollection<AllergenViewModel> GetAllergensWithId()
        {
            return this.allergenRepository.AllAsNoTracking().Select(x => new AllergenViewModel()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToList();
        }
    }
}
