namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class AddDishViewModel : IMapTo<Dish>, IHaveCustomMappings
    {
        [Required]
        public string Name { get; set; }

        // In Leva
        [Range(0, 5000, ErrorMessage = "The price cannot be less than 0 or more than 5000$!")]
        public decimal Price { get; set; }

        // In Grams
        [Range(0, 25000, ErrorMessage = "The weight cannot be less than 0 or more than 25 kilograms!")]
        public double Weight { get; set; }

        // In Minutes
        [Range(0, 5 * 60, ErrorMessage = "The prepare time cannot be less than 0 or more than 5 hours!")]
        [Display(Name = "Prepare time")]
        public int? PrepareTime { get; set; }

        [Required]
        [Display(Name = "Additional information")]
        [MinLength(1, ErrorMessage = "The additional info cannot be empty")]
        [MaxLength(255, ErrorMessage = "The additional info cannot be longer than 255 characters!")]
        public string AdditionalInfo { get; set; }

        [Required(ErrorMessage = "You must upload an image")]
        [NotMapped]
        public IFormFile Image { get; set; }

        [Display(Name = "Dish category")]
        public int DishTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> DishType { get; set; }

        [Display(Name = "Ingredients")]
        public List<int> IngredientsId { get; set; } = new List<int>();

        [IgnoreMap]
        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddDishViewModel, Dish>()
                .ForMember(x => x.Image, y => y.Ignore());
        }
    }
}
