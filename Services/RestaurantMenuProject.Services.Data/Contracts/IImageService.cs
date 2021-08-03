namespace RestaurantMenuProject.Services.Data.Contracts
{
    using RestaurantMenuProject.Data.Models;
    using System.Threading.Tasks;

    public interface IImageService
    {
        public Task<Image> AddImageAsync(string extension);

        //public void DeleteImage(string id);

        public Image GetImage(string id);
    }
}
