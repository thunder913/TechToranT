namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;

    public interface IImageService
    {
        public Image AddImage(string extension);

        public void DeleteImage(string id);

        public Image GetImage(string id);
    }
}
