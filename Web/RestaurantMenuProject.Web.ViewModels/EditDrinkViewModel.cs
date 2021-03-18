namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class EditDrinkViewModel : IMapTo<Drink>, IMapFrom<Drink>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, 5000)]
        public decimal Price { get; set; }

        [Range(0, 5000)]
        public double Weight { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string AdditionalInfo { get; set; }

        [DisplayName("Image")]
        public Image Image { get; set; }

        [DisplayName("New Image")]
        public IFormFile NewImage { get; set; }

        [Range(0, 100)]
        public decimal? AlchoholByVolume { get; set; }

        public int DrinkTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> DrinkType { get; set; }

        [DisplayName("Packaging type")]
        public int PackagingTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> PackagingType { get; set; }

        public List<int> IngredientsId { get; set; } = new List<int>();

        [IgnoreMap]
        public List<SelectListItem> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Drink, EditDrinkViewModel>()
                .ForMember(x => x.IngredientsId, y => y.MapFrom(x => x.Ingredients.Select(y => y.Id)));
        }
    }
}
