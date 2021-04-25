using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface ICommentService
    {
        public ICollection<CommentViewModel> GetCommentsForItem(int itemsPerPage, int page, string itemId);

        public int GetCommentsCountForItem(string itemId);

        public Task AddCommentAsync(string commentText, int rating, string userId, FoodType foodtype, string foodId);
    }
}
