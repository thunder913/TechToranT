namespace RestaurantMenuProject.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class EditDrinkViewModel : AddDrinkViewModel, IMapTo<Drink>, IMapFrom<Drink>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [DisplayName("Image")]
        public new Image Image { get; set; }

        [DisplayName("New Image")]
        public IFormFile NewImage { get; set; }

        public new void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Drink, EditDrinkViewModel>()
                .ForMember(x => x.IngredientsId, y => y.MapFrom(x => x.Ingredients.Select(y => y.Id)));
        }
    }
}
