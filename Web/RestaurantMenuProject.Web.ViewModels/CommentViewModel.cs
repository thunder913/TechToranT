namespace RestaurantMenuProject.Web.ViewModels
{
    using AutoMapper;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Mapping;
    using System;

    public class CommentViewModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string CommentText { get; set; }

        public string AuthorName { get; set; }

        public int LikesCount { get; set; }

        public int DislikesCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentViewModel>()
                .ForMember(x => x.AuthorName, y => y.MapFrom(x => x.CommentedBy.FirstName + " " + x.CommentedBy.LastName));
        }
    }
}
