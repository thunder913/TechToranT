namespace RestaurantMenuProject.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Web.ViewModels;

    public class DrinkService : IDrinkService
    {
        private readonly IDeletableEntityRepository<Drink> drinkRepository;
        private readonly IIngredientService ingredientService;
        private readonly IFileService fileService;
        private readonly IDrinkTypeService drinkTypeService;
        private readonly IImageService imageService;

        public DrinkService(
            IDeletableEntityRepository<Drink> drinkRepository,
            IIngredientService ingredientService,
            IFileService fileService,
            IDrinkTypeService drinkTypeService,
            IImageService imageService)
        {
            this.drinkRepository = drinkRepository;
            this.ingredientService = ingredientService;
            this.fileService = fileService;
            this.drinkTypeService = drinkTypeService;
            this.imageService = imageService;
        }

        public Task AddDrink(AddDrinkViewModel drink, string wwwroot)
        {
            var extension = drink.Image.FileName.Split(".")[^1];
            var image = this.imageService.AddImage(extension);

            var mapper = AutoMapperConfig.MapperInstance;
            var drinkToAdd = mapper.Map<Drink>(drink);
            drinkToAdd.ImageId = image.Id;
            drinkToAdd.Ingredients = this.ingredientService.GetAllIngredientsByIds(drink.IngredientsId.ToArray()).ToArray();

            this.drinkRepository.AddAsync(drinkToAdd).GetAwaiter().GetResult();
            this.drinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
            this.fileService.SaveImage("Drinks", image.Id, drink.Image, wwwroot, extension);

            return Task.CompletedTask;
        }

        public DrinkItemViewModel GetDrinkItemViewModelById(string id)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .To<DrinkItemViewModel>()
                    .First();
        }

        public Drink GetDrinkById(string id)
        {
            return this.drinkRepository
                .All()
                .Include(x => x.Ingredients)
                .FirstOrDefault(x => x.Id == id);
        }

        public ICollection<DrinkItemViewModel> GetAllDrinksByType(string drinkType)
        {
            return this.drinkRepository
                    .AllAsNoTracking()
                    .Where(x => x.DrinkType.Name == drinkType)
                    .To<DrinkItemViewModel>()
                    .ToList();
        }

        public EditDrinkViewModel GetEditDrinkViewModelById(string id)
        {
            return this.drinkRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<EditDrinkViewModel>()
                .FirstOrDefault();
        }

        public void EditDrink(EditDrinkViewModel editDrink, string wwwroot)
        {
            var drink = this.GetDrinkById(editDrink.Id);

            // Setting new values to the old dish
            drink.Name = editDrink.Name;
            drink.Price = editDrink.Price;
            drink.Weight = editDrink.Weight;
            drink.PackagingTypeId = editDrink.PackagingTypeId;
            drink.AlchoholByVolume = editDrink.AlchoholByVolume;
            drink.AdditionalInfo = editDrink.AdditionalInfo;
            drink.DrinkTypeId = editDrink.DrinkTypeId;

            // Removing the ingredients that are not containted in the new dish
            foreach (var ingr in drink.Ingredients)
            {
                if (!editDrink.IngredientsId.Contains(ingr.Id))
                {
                    drink.Ingredients.Remove(ingr);
                }
            }

            // Adding the ingredients that are not containted in the old dish
            foreach (var ingredientId in editDrink.IngredientsId)
            {
                if (!drink.Ingredients.Any(x => x.Id == ingredientId))
                {
                    var ingredient = this.ingredientService.GetIngredientById(ingredientId);
                    drink.Ingredients.Add(ingredient);
                }
            }

            // Checking if there is a new image
            if (editDrink.NewImage != null)
            {
                var oldImage = this.imageService.GetImage(drink.ImageId);
                this.fileService.DeleteImage($"{wwwroot}/img/Drinks/{oldImage.Id}.{oldImage.Extension}");
                var extension = editDrink.NewImage.FileName.Split(".")[^1];
                var image = this.imageService.AddImage(extension);
                this.fileService.SaveImage("Drinks", image.Id, editDrink.NewImage, wwwroot, extension);
                drink.ImageId = image.Id;
                drink.Image = image;
            }

            // Saving the new dish to the DB
            this.drinkRepository.Update(drink);
            this.drinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public void DeleteDrinkById(string id)
        {
            var drinkToRemove = this.drinkRepository.All().FirstOrDefault(x => x.Id == id);
            this.drinkRepository.Delete(drinkToRemove);
            this.drinkRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public Drink GetDrinkWithDeletedById(string id)
        {
            return this.drinkRepository
                .AllAsNoTrackingWithDeleted()
                .Include(x => x.Image)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
