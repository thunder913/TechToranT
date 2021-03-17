namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models.Enums;

    public class Image : BaseDeletableModel<string>
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ImageExtension Extension { get; set; }
    }
}
