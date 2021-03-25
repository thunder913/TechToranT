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

        [Range(0,255)]
        public int Capacity { get; set; }
    }
}
