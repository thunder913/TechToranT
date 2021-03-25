namespace RestaurantMenuProject.Data.Models.Dtos
{
    public class EditStatusDto
    {
        public int NewProcessingTypeId { get; set; }

        public string OrderId { get; set; }

        public string OldProcessingType { get; set; }
    }
}
