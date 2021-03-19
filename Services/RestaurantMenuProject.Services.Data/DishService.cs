namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DishService : IDishService
    {
        private readonly IDeletableEntityRepository<Dish> dishRepository;
        private readonly IIngredientService ingredientService;
        private readonly IFileService fileService;
        private readonly IDishTypeService dishTypeService;
        private readonly IImageService imageService;

        public DishService(
            IDeletableEntityRepository<Dish> dishRepository,
            IIngredientService ingredientService,
            IFileService fileService,
            IDishTypeService dishTypeService,
            IImageService imageService)
        {
            this.dishRepository = dishRepository;
            this.ingredientService = ingredientService;
            this.fileService = fileService;
            this.dishTypeService = dishTypeService;
            this.imageService = imageService;
        }

        public Task AddDish(AddDishViewModel dish, string wwwroot)
        {
            var extension = dish.Image.FileName.Split(".")[^1];
            var image = this.imageService.AddImage(extension);

            var mapper = AutoMapperConfig.MapperInstance;
            var dishToAdd = mapper.Map<Dish>(dish);
            dishToAdd.ImageId = image.Id;
            dishToAdd.Ingredients = this.ingredientService.GetAllIngredientsByIds(dish.IngredientsId.ToArray()).ToArray();

            this.dishRepository.AddAsync(dishToAdd).GetAwaiter().GetResult();
            this.dishRepository.SaveChangesAsync().GetAwaiter().GetResult();
            this.fileService.SaveImage("Dishes", image.Id, dish.Image, wwwroot, extension);

            return Task.CompletedTask;
        }

        public void RemoveDish(Dish dish)
        {
            this.dishRepository.Delete(dish);
        }

        public Dish GetDishById(string id)
        {
            return this.dishRepository.All().Include(x => x.Ingredients).Where(x => x.Id == id).FirstOrDefault();
        }

        public DishViewModel GetDishAsFoodItemById(string id)
        {
            return this.dishRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .To<DishViewModel>()
                    .FirstOrDefault();
        }

        public ICollection<DishViewModel> GetAllDisheshWithDishTypeAsFoodItem(string dishType)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.DishType.Name == dishType)
                        .To<DishViewModel>()
                        .ToList();
        }

        public EditDishViewModel GetEditDishViewModelById(string id)
        {
            return this.dishRepository
                        .AllAsNoTracking()
                        .Where(x => x.Id == id)
                        .To<EditDishViewModel>()
                        .FirstOrDefault();
        }

        public void EditDish(EditDishViewModel editDish, string wwwroot)
        {
            var dish = this.GetDishById(editDish.Id);

            // Setting new values to the old dish
            dish.Name = editDish.Name;
            dish.Price = editDish.Price;
            dish.Weight = editDish.Weight;
            dish.PrepareTime = editDish.PrepareTime;
            dish.AdditionalInfo = editDish.AdditionalInfo;
            dish.DishTypeId = editDish.DishTypeId;

            // Removing the ingredients that are not containted in the new dish
            foreach (var ingr in dish.Ingredients)
            {
                if (!editDish.IngredientsId.Contains(ingr.Id))
                {
                    dish.Ingredients.Remove(ingr);
                }
            }

            // Adding the ingredients that are not containted in the old dish

            foreach (var ingredientId in editDish.IngredientsId)
            {
                if (!dish.Ingredients.Any(x => x.Id == ingredientId))
                {
                    var ingredient = this.ingredientService.GetIngredientById(ingredientId);
                    dish.Ingredients.Add(ingredient);
                }
            }

            // Checking if there is a new image
            if (editDish.NewImage != null)
            {
                var oldImage = this.imageService.GetImage(dish.ImageId);
                this.fileService.DeleteImage($"{wwwroot}/img/Dishes/{oldImage.Id}.{oldImage.Extension}");
                var extension = editDish.NewImage.FileName.Split(".")[^1];
                var image = this.imageService.AddImage(extension);
                this.fileService.SaveImage("Dishes", image.Id, editDish.NewImage, wwwroot, extension);
                dish.ImageId = image.Id;
                dish.Image = image;
            }

            // Saving the new dish to the DB
            this.dishRepository.Update(dish);
            this.dishRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private ICollection<Ingredient> GetDishIngredients(string id)
        {
            return this.dishRepository.All().First(x => x.Id == id).Ingredients;
        }
    }
}
