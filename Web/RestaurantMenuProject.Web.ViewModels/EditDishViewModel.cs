namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class EditDishViewModel : IMapTo<Dish>, IMapFrom<Dish>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        // In Leva
        [Range(0, 5000)]
        public decimal Price { get; set; }

        // In Grams
        [Range(0, 25000)]
        public double Weight { get; set; }

        // In Minutes
        [Range(0, 3 * 60)]
        [DisplayName("Prepare Time")]
        public int? PrepareTime { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        [DisplayName("Additional Information:")]
        public string AdditionalInfo { get; set; }

        [DisplayName("Image")]
        public Image Image { get; set; }

        [DisplayName("New Image")]
        public IFormFile NewImage { get; set; }

        public int DishTypeId { get; set; }

        [IgnoreMap]
        public List<SelectListItem> DishType { get; set; }

        [DisplayName("Ingredients")]
        public List<int> IngredientsId { get; set; } = new List<int>();

        [IgnoreMap]
        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Dish, EditDishViewModel>()
                .ForMember(x => x.IngredientsId, y => y.MapFrom(x => x.Ingredients.Select(y => y.Id)));
        }
    }
}
