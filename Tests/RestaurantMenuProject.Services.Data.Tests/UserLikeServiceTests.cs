using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Services.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class UserLikeServiceTests : BaseServiceTests
    {
        private IUserLikeService UserLikeService => this.ServiceProvider.GetRequiredService<IUserLikeService>();

        [Fact]
        public async Task AddLikeToCommentAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var commentId = 1;
            var userId = "user1";

            await this.UserLikeService.AddLikeToCommentAsync(userId, commentId);

            var like = this.DbContext.UsersLikes.FirstOrDefault();

            Assert.Equal(userId, like.UserId);
            Assert.Equal(commentId, like.CommentId);
        }

        [Fact]
        public async Task HasUserLikedACommentWorksCorrectly()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserLikeService.AddLikeToCommentAsync(userId, commentId);

            var commentOne = this.UserLikeService.HasUserLikedAComment(userId, commentId);
            var commentTwo = this.UserLikeService.HasUserLikedAComment(userId, 2);

            Assert.True(commentOne);
            Assert.False(commentTwo);
        }

        [Fact]
        public async Task RemoveLikeAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserLikeService.AddLikeToCommentAsync(userId, commentId);
            await this.UserLikeService.RemoveLikeAsync(userId, commentId);

            var like = this.DbContext.UsersLikes.FirstOrDefault();
            Assert.Null(like);
        }

        [Fact]
        public async Task RemoveLikeAsyncThrowsWhenGivenInvalidData()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserLikeService.AddLikeToCommentAsync(userId, commentId);
            await Assert.ThrowsAsync<Exception>(async () => await this.UserLikeService.RemoveLikeAsync(userId + "invalid", commentId));
            await Assert.ThrowsAsync<Exception>(async () => await this.UserLikeService.RemoveLikeAsync(userId, commentId + 99));
            await Assert.ThrowsAsync<Exception>(async () => await this.UserLikeService.RemoveLikeAsync("user2", 3));
        }
    }
}
