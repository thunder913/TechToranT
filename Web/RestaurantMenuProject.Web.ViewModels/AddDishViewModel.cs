namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class AddDishViewModel : IMapTo<Dish>
    {
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
        public int? PrepareTime { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string AdditionalInfo { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public int DishTypeId { get; set; }

        public List<SelectListItem> DishType { get; set; }

        public List<int> IngredientsId { get; set; } = new List<int>();

        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();
    }
}
