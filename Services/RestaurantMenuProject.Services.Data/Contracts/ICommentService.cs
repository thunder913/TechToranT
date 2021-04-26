using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface ICommentService
    {
        public Task<ICollection<CommentViewModel>> GetCommentsForItemAsync(int itemsPerPage, int page, string itemId, string userId);

        public int GetCommentsCountForItem(string itemId);

        public Task AddCommentAsync(string commentText, int rating, string userId, FoodType foodtype, string foodId);

        public Task DeleteCommentByIdAsync(int commentId, string userId);
    }
}
