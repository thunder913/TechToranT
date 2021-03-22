namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class DataTableDto
    {
        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public DataTableSearchDto Order { get; set; }
    }
}
