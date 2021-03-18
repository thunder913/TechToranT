using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using System;
using System.Linq;

namespace RestaurantMenuProject.Services.Data
{
    public class ImageService : IImageService
    {
        private readonly IDeletableEntityRepository<Image> imageRepository;

        public ImageService(IDeletableEntityRepository<Image> imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        public Image AddImage(string extension)
        {
            var image = new Image()
            {
                Extension = (ImageExtension)Enum.Parse(typeof(ImageExtension), extension),
            };
            this.imageRepository.AddAsync(image).GetAwaiter().GetResult();
            this.imageRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return image;
        }

        public void DeleteImage(string id)
        {
            var image = this.imageRepository.All().First(x => x.Id == id);
            this.imageRepository.Delete(image);
        }

        public Image GetImage(string id)
        {
            return this.imageRepository.All().FirstOrDefault(x => x.Id == id);
        }
    }
}
