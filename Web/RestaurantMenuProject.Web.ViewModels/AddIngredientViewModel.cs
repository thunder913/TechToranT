namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddIngredientViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Allergens")]
        public List<int> AllergensId { get; set; } = new List<int>();

        public List<SelectListItem> Allergens { get; set; }
    }
}
