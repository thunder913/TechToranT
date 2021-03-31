namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class AllergenService : IAllergenService
    {
        private readonly IDeletableEntityRepository<Allergen> allergenRepository;

        public AllergenService(IDeletableEntityRepository<Allergen> allergenRepository)
        {
            this.allergenRepository = allergenRepository;
        }

        public async Task AddAllergenAsync(AllergenViewModel allergen)
        {
            var allergenToAdd = new Allergen()
            {
                Name = allergen.Name,
            };
            await this.allergenRepository.AddAsync(allergenToAdd);
            await this.allergenRepository.SaveChangesAsync();
        }

        public ICollection<AllergenViewModel> GetAllergensWithId()
        {
            return this.allergenRepository
                .AllAsNoTracking()
                .To<AllergenViewModel>()
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
