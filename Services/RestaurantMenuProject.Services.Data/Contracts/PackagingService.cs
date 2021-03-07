namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Web.ViewModels;

    public class PackagingService : IPackagingService
    {
        private readonly IDeletableEntityRepository<PackagingType> packagingRepository;

        public PackagingService(IDeletableEntityRepository<PackagingType> packagingRepository)
        {
            this.packagingRepository = packagingRepository;
        }

        public ICollection<FoodTypeViewModel> GetAllPackagingTypes()
        {
            return this.packagingRepository.AllAsNoTracking().Select(x => new FoodTypeViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            })
        .ToList();
        }
    }
}
