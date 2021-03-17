namespace RestaurantMenuProject.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddItemToBasketViewModel
    {
        [Required]
        public string Id { get; set; }

        [Range(1, 1000)]
        public int Count { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
