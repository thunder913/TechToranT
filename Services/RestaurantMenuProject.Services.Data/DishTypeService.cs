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

    public class DishTypeService : IDishTypeService
    {
        private readonly IDeletableEntityRepository<DishType> dishTypeRepository;
        private readonly IImageService imageService;
        private readonly IFileService fileService;

        public DishTypeService(
            IDeletableEntityRepository<DishType> dishTypeRepository,
            IImageService imageService,
            IFileService fileService)
        {
            this.dishTypeRepository = dishTypeRepository;
            this.imageService = imageService;
            this.fileService = fileService;
        }

        public ICollection<MenuItemViewModel> GetAllDishTypes()
        {
            return this.dishTypeRepository
                .AllAsNoTracking()
                .To<MenuItemViewModel>()
                .ToList();
        }

        public ICollection<FoodTypeViewModel> GetAllDishTypesWithId()
        {
            return this.dishTypeRepository
                .AllAsNoTracking()
                .To<FoodTypeViewModel>()
                .ToList();
        }

        public DishType GetDishTypeById(int id)
        {
            return this.dishTypeRepository
                .All()
                .Include(x => x.Image)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task AddDishTypeAsync(AddCategoryViewModel dishCategory, string wwwroot)
        {
            var extension = dishCategory.Image.FileName.Split(".")[^1];
            var image = await this.imageService.AddImageAsync(extension);

            var dishType = new DishType()
            {
                Name = dishCategory.Name,
                Image = image,
                Description = dishCategory.Description,
            };

            await this.fileService.SaveImageAsync("Menu", image.Id, dishCategory.Image, wwwroot, extension);
            await this.dishTypeRepository.AddAsync(dishType);
            await this.dishTypeRepository.SaveChangesAsync();
        }

        public async Task EditDishTypeAsync(EditCategoryViewModel editCategory, string wwwroot)
        {
            var dishType = this.GetDishTypeById(editCategory.Id);

            dishType.Name = editCategory.Name;
            dishType.Description = editCategory.Description;

            if (editCategory.NewImage != null)
            {
                var oldImage = this.imageService.GetImage(dishType.Image.Id);
                this.fileService.DeleteImage($"{wwwroot}/img/Menu/{oldImage.Id}.{oldImage.Extension}");
                var extension = editCategory.NewImage.FileName.Split(".")[^1];
                var image = await this.imageService.AddImageAsync(extension);
                await this.fileService.SaveImageAsync("Menu", image.Id, editCategory.NewImage, wwwroot, extension);
                dishType.Image = image;
            }

            this.dishTypeRepository.Update(dishType);
            await this.dishTypeRepository.SaveChangesAsync();
        }

        public async Task DeleteDishTypeAsync(int id)
        {
            var typeToDelete = this.GetDishTypeById(id);
            this.dishTypeRepository.Delete(typeToDelete);
            await this.dishTypeRepository.SaveChangesAsync();
        }

        public ICollection<DishType> GetAllDishTypesWithIds(int[] ids)
        {
            return this.dishTypeRepository
                .All()
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }
    }
}
