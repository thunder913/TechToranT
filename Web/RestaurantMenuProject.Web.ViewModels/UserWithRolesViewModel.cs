namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Linq;

    using AutoMapper;
    using Newtonsoft.Json;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;

    public class UserWithRolesViewModel : IMapFrom<ApplicationUser>, IMapTo<UserViewModel>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string[] RoleIds { get; set; }

        [IgnoreMap]
        public string Roles { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserWithRolesViewModel>()
                .ForMember(x => x.RoleIds, y => y.MapFrom(x => x.Roles.Select(r => r.RoleId)))
                .ForMember(x => x.Name, y => y.MapFrom(x => x.FirstName + " " + x.LastName));
        }
    }
}
