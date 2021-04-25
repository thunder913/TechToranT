namespace RestaurantMenuProject.Web.ViewModels
{
    using RestaurantMenuProject.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class AddCommentViewModel
    {
        [Required]
        [MaxLength(127)]
        public string Comment { get; set; }

        public int Rating { get; set; }

        public FoodType FoodType { get; set; }

        [Required]
        public string FoodId { get; set; }
    }
}
