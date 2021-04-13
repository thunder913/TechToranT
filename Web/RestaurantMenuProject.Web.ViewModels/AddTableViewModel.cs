namespace RestaurantMenuProject.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class AddTableViewModel : IMapFrom<Table>, IMapTo<Table>
    {
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [Range(1, 255, ErrorMessage = "The tables capacity must be between 1 and 255!")]
        public int Capacity { get; set; }
    }
}
