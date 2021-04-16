namespace RestaurantMenuProject.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkTypeService : IDrinkTypeService
    {
        private readonly IDeletableEntityRepository<DrinkType> drinkTypeRepository;
        private readonly IImageService imageService;
        private readonly IFileService fileService;

        public DrinkTypeService(
            IDeletableEntityRepository<DrinkType> drinkTypeRepository,
            IImageService imageService,
            IFileService fileService)
        {
            this.drinkTypeRepository = drinkTypeRepository;
            this.imageService = imageService;
            this.fileService = fileService;
        }

        public ICollection<MenuItemViewModel> GetAllDrinkTypes()
        {
            return this.drinkTypeRepository
                .All()
                .To<MenuItemViewModel>()
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDrinkTypesWithId()
        {
            return this.drinkTypeRepository.AllAsNoTracking()
                .To<FoodTypeViewModel>()
                .ToList();
        }

        public DrinkType GetDrinkTypeById(int id)
        {
            return this.drinkTypeRepository
                    .All()
                    .Include(x => x.Image)
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
        }

        public async Task AddDrinkTypeAsync(AddCategoryViewModel drinkCategory, string wwwroot)
        {
            var extension = drinkCategory.Image.FileName.Split(".")[^1];
            var image = await this.imageService.AddImageAsync(extension);

            var drinkType = new DrinkType()
            {
                Name = drinkCategory.Name,
                Image = image,
                Description = drinkCategory.Description,
            };

            await this.fileService.SaveImageAsync("Menu", image.Id, drinkCategory.Image, wwwroot, extension);
            await this.drinkTypeRepository.AddAsync(drinkType);
            await this.drinkTypeRepository.SaveChangesAsync();
        }

        public async Task EditDrinkTypeAsync(EditCategoryViewModel editCategory, string wwwroot)
        {
            var drinkType = this.GetDrinkTypeById(editCategory.Id);

            drinkType.Name = editCategory.Name;
            drinkType.Description = editCategory.Description;

            if (editCategory.NewImage != null)
            {
                var oldImage = this.imageService.GetImage(drinkType.Image.Id);
                this.fileService.DeleteImage($"{wwwroot}/img/Menu/{oldImage.Id}.{oldImage.Extension}");
                var extension = editCategory.NewImage.FileName.Split(".")[^1];
                var image = await this.imageService.AddImageAsync(extension);
                await this.fileService.SaveImageAsync("Menu", image.Id, editCategory.NewImage, wwwroot, extension);
                drinkType.Image = image;
            }

            this.drinkTypeRepository.Update(drinkType);
            await this.drinkTypeRepository.SaveChangesAsync();
        }

        public async Task DeleteDrinkTypeAsync(int id)
        {
            var typeToDelete = this.GetDrinkTypeById(id);
            this.drinkTypeRepository.Delete(typeToDelete);
            await this.drinkTypeRepository.SaveChangesAsync();
        }

        public ICollection<DrinkType> GetAllDrinkTypesWithIds(int[] ids)
        {
            return this.drinkTypeRepository
                .All()
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }
    }
}