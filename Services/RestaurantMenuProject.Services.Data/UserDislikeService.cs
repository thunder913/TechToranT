using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class UserDislikeService : IUserDislikeService
    {
        private readonly IRepository<UserDislike> dislikeRepository;

        public UserDislikeService(IRepository<UserDislike> dislikeRepository)
        {
            this.dislikeRepository = dislikeRepository;
        }

        public async Task AddDislikeToCommentAsync(string userId, int commentId)
        {
            var userLike = new UserDislike()
            {
                UserId = userId,
                CommentId = commentId,
            };

            await this.dislikeRepository.AddAsync(userLike);
            await this.dislikeRepository.SaveChangesAsync();
        }

        public bool HasUserDislikedAComment(string userId, int commentId)
        {
            return this.dislikeRepository.All().Any(x => x.UserId == userId && x.CommentId == commentId);
        }

        public async Task RemoveDislikeAsync(string userId, int commentId)
        {
            var userLike = this.dislikeRepository.All().FirstOrDefault(x => x.UserId == userId && x.CommentId == commentId);
            if (userLike == null)
            {
                throw new System.Exception("Something went wrong, try again!");
            }

            this.dislikeRepository.Delete(userLike);
            await this.dislikeRepository.SaveChangesAsync();
        }
    }
}
