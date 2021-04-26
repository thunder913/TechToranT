using Microsoft.EntityFrameworkCore;
using RestaurantMenuProject.Common;
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
        private readonly IUserService userService;

        public CommentService(
            IDeletableEntityRepository<Comment> commentRepository,
            IUserService userService)
        {
            this.commentRepository = commentRepository;
            this.userService = userService;
        }

        public async Task<ICollection<CommentViewModel>> GetCommentsForItemAsync(int itemsPerPage, int page, string itemId, string userId)
        {
            var comments = this.commentRepository
                    .AllAsNoTracking()
                    .Where(x => x.DishId == itemId || x.DrinkId == itemId)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .To<CommentViewModel>()
                    .ToList();
            foreach (var comment in comments)
            {
                if (comment.CommentedById == userId || await this.userService.IsUserInTheRole(userId, GlobalConstants.AdministratorRoleName))
                {
                    comment.IsCommenter = true;
                }
            }

            return comments;
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

        public async Task DeleteCommentByIdAsync(int commentId, string userId)
        {
            var comment = this.commentRepository.All().FirstOrDefault(x => x.Id == commentId);
            if (comment.CommentedById == userId || await this.userService.IsUserInTheRole(userId, GlobalConstants.AdministratorRoleName))
            {
                this.commentRepository.Delete(comment);
                await this.commentRepository.SaveChangesAsync();
            }
            else
            {
                throw new System.Exception("User is not permitted to delete the comment!");
            }


        }
    }
}
