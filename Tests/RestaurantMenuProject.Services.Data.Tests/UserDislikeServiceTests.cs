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
    public class UserDislikeServiceTests : BaseServiceTests
    {
        private IUserDislikeService UserDislikeService => this.ServiceProvider.GetRequiredService<IUserDislikeService>();

        [Fact]
        public async Task AddDislikeToCommentAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var commentId = 1;
            var userId = "user1";

            await this.UserDislikeService.AddDislikeToCommentAsync(userId, commentId);

            var dislike = this.DbContext.UsersDislikes.FirstOrDefault();

            Assert.Equal(userId, dislike.UserId);
            Assert.Equal(commentId, dislike.CommentId);
        }

        [Fact]
        public async Task HasUserDislikedACommentWorksCorrectly()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserDislikeService.AddDislikeToCommentAsync(userId, commentId);

            var commentOne = this.UserDislikeService.HasUserDislikedAComment(userId, commentId);
            var commentTwo = this.UserDislikeService.HasUserDislikedAComment(userId, 2);

            Assert.True(commentOne);
            Assert.False(commentTwo);
        }

        [Fact]
        public async Task RemoveDislikeAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserDislikeService.AddDislikeToCommentAsync(userId, commentId);
            await this.UserDislikeService.RemoveDislikeAsync(userId, commentId);

            var dislike = this.DbContext.UsersDislikes.FirstOrDefault();
            Assert.Null(dislike);
        }

        [Fact]
        public async Task RemoveDislikeAsyncThrowsWhenGivenInvalidData()
        {
            await this.PopulateDB();
            var userId = "user1";
            var commentId = 1;

            await this.UserDislikeService.AddDislikeToCommentAsync(userId, commentId);
            await Assert.ThrowsAsync<Exception>(async () => await this.UserDislikeService.RemoveDislikeAsync(userId + "invalid", commentId));
            await Assert.ThrowsAsync<Exception>(async () => await this.UserDislikeService.RemoveDislikeAsync(userId, commentId + 99));
            await Assert.ThrowsAsync<Exception>(async () => await this.UserDislikeService.RemoveDislikeAsync("user2", 3));
        }
    }
}
