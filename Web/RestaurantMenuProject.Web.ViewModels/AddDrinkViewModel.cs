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

        [Range(0, 5000)]
        public decimal Price { get; set; }

        [Range(0, 5000)]
        public double Weight { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string AdditionalInfo { get; set; }

        [Range(0, 100)]
        public decimal? AlchoholByVolume { get; set; }

        public int DrinkTypeId { get; set; }

        public List<SelectListItem> DrinkType { get; set; }

        [Required]
        [NotMapped]
        public IFormFile Image { get; set; }

        [DisplayName("Packaging type")]
        public int PackagingTypeId { get; set; }

        public List<SelectListItem> PackagingType { get; set; }

        public List<int> IngredientsId { get; set; } = new List<int>();

        public List<SelectListItem> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddDrinkViewModel, Drink>()
                .ForMember(x => x.Image, y => y.Ignore());
        }
    }
}
