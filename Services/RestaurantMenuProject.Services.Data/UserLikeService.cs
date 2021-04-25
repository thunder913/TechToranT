namespace RestaurantMenuProject.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Services.Data.Contracts;

    public class UserLikeService : IUserLikeService
    {
        private readonly IRepository<UserLike> likeRepository;

        public UserLikeService(IRepository<UserLike> likeRepository)
        {
            this.likeRepository = likeRepository;
        }

        public async Task AddLikeToCommentAsync(string userId, int commentId)
        {
            var userLike = new UserLike()
            {
                UserId = userId,
                CommentId = commentId,
            };

            await this.likeRepository.AddAsync(userLike);
            await this.likeRepository.SaveChangesAsync();
        }

        public bool HasUserLikedAComment(string userId, int commentId)
        {
            return this.likeRepository.All().Any(x => x.UserId == userId && x.CommentId == commentId);
        }

        public async Task RemoveLikeAsync(string userId, int commentId)
        {
            var userLike = this.likeRepository.All().FirstOrDefault(x => x.UserId == userId && x.CommentId == commentId);
            if (userLike == null)
            {
                throw new System.Exception("Something went wrong, try again!");
            }

            this.likeRepository.Delete(userLike);
            await this.likeRepository.SaveChangesAsync();
        }
    }
}
