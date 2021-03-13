namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IFileService
    {
        public Task SaveImage(string itemCategory, string type, int id, IFormFile formFile, string wwwroot);
    }
}
