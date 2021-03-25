namespace RestaurantMenuProject.Data.Models
{
    using RestaurantMenuProject.Data.Common.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Table : BaseDeletableModel<int>
    {
        [Required]
        public int Number { get; set; }

        public int Capacity { get; set; }

        [Required]
        public string Code { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
