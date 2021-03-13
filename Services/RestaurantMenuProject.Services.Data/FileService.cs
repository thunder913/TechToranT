using Microsoft.AspNetCore.Http;
using RestaurantMenuProject.Services.Data.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class FileService : IFileService
    {
        public async Task SaveImage(string itemCategory, string type, int id, IFormFile formFile, string wwwroot)
        {
            var path = $"{wwwroot}/img/{itemCategory}/{type}";

            Directory.CreateDirectory(path);

            using (FileStream fs = new FileStream(path + $"/{id}.jpg", FileMode.Create))
            {
                await formFile.CopyToAsync(fs);
            }
        }
    }
}
