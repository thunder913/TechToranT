using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class AllergenViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
