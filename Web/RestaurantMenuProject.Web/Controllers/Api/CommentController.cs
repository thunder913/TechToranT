namespace RestaurantMenuProject.Web.Controllers.Api
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;

    [Route("api/[Controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly IUserLikeService userLikeService;
        private readonly IUserDislikeService userDislikeService;

        public CommentController(ICommentService commentService,
            IUserLikeService userLikeService,
            IUserDislikeService userDislikeService)
        {
            this.commentService = commentService;
            this.userLikeService = userLikeService;
            this.userDislikeService = userDislikeService;
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpGet("GetComments")]
        public AllCommentsViewModel GetComments([FromQuery] GetCommentsViewModel viewModel)
        {
            var commentsPerPage = 10;

            var comments = new AllCommentsViewModel()
            {
                Page = viewModel.Page,
                CommentCount = this.commentService.GetCommentsCountForItem(viewModel.ItemId),
                CommentPerPage = commentsPerPage,
                Comments = this.commentService.GetCommentsForItem(commentsPerPage, viewModel.Page, viewModel.ItemId),
            };
            return comments;
        }

        [HttpPost("AddComment")]
        public async Task<bool> AddComment(AddCommentViewModel comment)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.commentService.AddCommentAsync(comment.Comment, comment.Rating, userId, comment.FoodType, comment.FoodId);

            return true;
        }

        [HttpPost("AddLike")]
        public async Task<string> AddLike([FromBody] int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (this.userDislikeService.HasUserDislikedAComment(userId, id))
            {
                return "You cannot like a comment you have already disliked!";
            }
            else if (this.userLikeService.HasUserLikedAComment(userId, id))
            {
                await this.userLikeService.RemoveLikeAsync(userId, id);
                return "Successfuly removed your like from the comment!";
            }

            await this.userLikeService.AddLikeToCommentAsync(userId, id);
            return "Successfully liked the comment!";
        }

        [HttpPost("AddDislike")]
        public async Task<string> AddDislike([FromBody] int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (this.userLikeService.HasUserLikedAComment(userId, id))
            {
                return "You cannot dislike a comment you have already liked!";
            }
            else if (this.userDislikeService.HasUserDislikedAComment(userId, id))
            {
                await this.userDislikeService.RemoveDislikeAsync(userId, id);
                return "Successfuly removed your dislike from the comment!";
            }

            await this.userDislikeService.AddDislikeToCommentAsync(userId, id);
            return "Successfully disliked the comment!";
        }
    }
}
