using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class EditUserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public List<string> RoleIds { get; set; } = new List<string>();

        [IgnoreMap]
        public List<SelectListItem> Roles { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, EditUserViewModel>()
                .ForMember(x => x.RoleIds, y => y.MapFrom(x => x.Roles.Select(z => z.RoleId).ToList()));
        }
    }
}
