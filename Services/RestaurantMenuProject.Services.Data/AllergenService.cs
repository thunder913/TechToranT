namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    public class AllergenService : IAllergenService
    {
        private readonly IDeletableEntityRepository<Allergen> allergenRepository;

        public AllergenService(IDeletableEntityRepository<Allergen> allergenRepository)
        {
            this.allergenRepository = allergenRepository;
        }

        public async Task AddAllergen(Allergen allergen)
        {
            await this.allergenRepository.AddAsync(allergen);
            await this.allergenRepository.SaveChangesAsync();
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

        public ICollection<Allergen> GetAllergensWithIds(List<int> ids)
        {
            return this.allergenRepository
                        .All()
                        .Where(x => ids.Contains(x.Id))
                        .ToList();
        }
    }
}
