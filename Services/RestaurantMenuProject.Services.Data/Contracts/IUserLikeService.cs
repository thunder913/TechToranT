using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IUserLikeService
    {
        public Task AddLikeToCommentAsync(string userId, int commentId);

        public bool HasUserLikedAComment(string userId, int commentId);

        public Task RemoveLikeAsync(string userId, int commentId);
    }
}
