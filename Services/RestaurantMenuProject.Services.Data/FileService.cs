namespace RestaurantMenuProject.Services.Data
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class FileService : IFileService
    {
        public async Task SaveImageAsync(string itemCategory, string id, IFormFile formFile, string wwwroot, string extension)
        {
            var path = $"{wwwroot}/img/{itemCategory}";

            Directory.CreateDirectory(path);

            using (FileStream fs = new FileStream(path + $"/{id}.{extension}", FileMode.Create))
            {
                await formFile.CopyToAsync(fs);
            }
        }

        public void DeleteImage(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
