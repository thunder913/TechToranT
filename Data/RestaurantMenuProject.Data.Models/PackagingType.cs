using RestaurantMenuProject.Data.Common.Models;

namespace RestaurantMenuProject.Data.Models
{
    public class PackagingType : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsEco { get; set; }
    }
}
