namespace RestaurantMenuProject.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "The email is invalid!")]
        public string Email { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "The phone number is invalid!")]
        public string Phone { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "The message must be at least 20 characters long!")]
        public string Message { get; set; }
    }
}
