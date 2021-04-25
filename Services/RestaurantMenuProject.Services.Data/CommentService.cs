using Microsoft.EntityFrameworkCore;
using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly IDeletableEntityRepository<Comment> commentRepository;

        public CommentService(IDeletableEntityRepository<Comment> commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public ICollection<CommentViewModel> GetCommentsForItem(int itemsPerPage, int page, string itemId)
        {
            return this.commentRepository
                    .AllAsNoTracking()
                    .Where(x => x.DishId == itemId || x.DrinkId == itemId)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .To<CommentViewModel>()
                    .ToList();
        }

        public int GetCommentsCountForItem(string itemId)
        {
            return this.commentRepository
                .AllAsNoTracking()
                .Where(x => x.DishId == itemId || x.DrinkId == itemId)
                .Count();
        }

        public async Task AddCommentAsync(string commentText, int rating, string userId, FoodType foodtype, string foodId)
        {
            var comment = new Comment()
            {
                CommentedById = userId,
                CommentText = commentText,
                Rating = rating,
            };

            if (foodtype == FoodType.Dish)
            {
                comment.DishId = foodId;
            }
            else if (foodtype == FoodType.Drink)
            {
                comment.DrinkId = foodId;
            }

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();
        }
    }
}
