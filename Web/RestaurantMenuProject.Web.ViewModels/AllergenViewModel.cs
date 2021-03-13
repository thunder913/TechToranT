namespace RestaurantMenuProject.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class AllergenViewModel : IMapFrom<Allergen>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
