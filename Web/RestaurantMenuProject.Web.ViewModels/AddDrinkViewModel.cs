namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class AddDrinkViewModel : IMapTo<Drink>, IHaveCustomMappings
    {
        [Required]
        public string Name { get; set; }

        [Range(0, 5000, ErrorMessage = "The price must be between 0 and 5000$!")]
        public decimal Price { get; set; }

        [Range(0, 5000, ErrorMessage = "The weight must be between 0 and 5 litres!")]
        public double Weight { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "The additional info cannote be empty!")]
        [MaxLength(255, ErrorMessage = "The additional information cannot be more than 255 characters long!")]
        [Display(Name = "Additional information")]
        public string AdditionalInfo { get; set; }

        [Range(0, 100, ErrorMessage = "The alcohol volume must be between 0 and 100!")]
        [Display(Name = "Alcohol volume")]
        public decimal? AlchoholByVolume { get; set; }

        [Display(Name = "Drink category")]
        public int DrinkTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> DrinkType { get; set; }

        [Required]
        [NotMapped]
        public IFormFile Image { get; set; }

        [Display(Name = "Packaging type")]
        public int PackagingTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> PackagingType { get; set; }

        [Display(Name = "Ingredients")]
        public List<int> IngredientsId { get; set; } = new List<int>();

        [IgnoreMap]
        public List<SelectListItem> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddDrinkViewModel, Drink>()
                .ForMember(x => x.Image, y => y.Ignore());
        }
    }
}
