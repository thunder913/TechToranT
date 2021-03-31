namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IFileService
    {
        public Task SaveImageAsync(string itemCategory, string id, IFormFile formFile, string wwwroot, string extension);

        public void DeleteImage(string path);
    }
}
