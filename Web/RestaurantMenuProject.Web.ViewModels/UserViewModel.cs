using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Mapping;
using System;
using System.Linq;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class UserViewModel : IMapFrom<UserWithRolesViewModel>, IHaveCustomMappings
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public UserViewModel()
        {
        }

        public UserViewModel(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Roles { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserWithRolesViewModel, UserViewModel>()
                .ForMember(x => x.Roles, y => y.MapFrom(x => string.Join(", ", x.RoleIds.Select(r => this.roleManager.FindByIdAsync(r).GetAwaiter().GetResult().Name))));
        }
    }
}
