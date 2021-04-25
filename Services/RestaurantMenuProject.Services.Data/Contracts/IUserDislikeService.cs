using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IUserDislikeService
    {
        public Task AddDislikeToCommentAsync(string userId, int commentId);

        public bool HasUserDislikedAComment(string userId, int commentId);

        public Task RemoveDislikeAsync(string userId, int commentId);
    }
}
