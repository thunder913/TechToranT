namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class EditDishViewModel : AddDishViewModel, IMapTo<Dish>, IMapFrom<Dish>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "You must upload an image")]
        public new Image Image { get; set; }

        [DisplayName("New Image")]
        public IFormFile NewImage { get; set; }

        public new void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Dish, EditDishViewModel>()
                .ForMember(x => x.IngredientsId, y => y.MapFrom(x => x.Ingredients.Select(y => y.Id)));
        }
    }
}
