namespace RestaurantMenuProject.Web.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;
    using RestaurantMenuProject.Data.Models;

    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Image")]
        public Image Image { get; set; }

        [DisplayName("New Image")]
        public IFormFile NewImage { get; set; }
    }
}
