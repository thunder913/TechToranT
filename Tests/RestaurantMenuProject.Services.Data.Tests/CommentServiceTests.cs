namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DeepEqual.Syntax;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using Xunit;

    public class CommentServiceTests : BaseServiceTests
    {
        private ICommentService CommentService => this.ServiceProvider.GetRequiredService<ICommentService>();

        [InlineData("test1", "user2", 1, 3)]
        [InlineData("test1", "user1", 1, 2)]
        [InlineData("test3", "user1", 2, 2)]
        [InlineData("test1", "user2", 5, 3)]
        [Theory]
        public async Task GetCommentsForItemAsyncWorksCorrectly(string itemId, string userId, int page, int itemsPerPage)
        {
            await this.PopulateDB();

            var expected = (await this.CommentService.GetCommentsForItemAsync(itemsPerPage, page, itemId, userId)).ToList();
            var actual = this.DbContext
                .Comments
                .Include(x => x.CommentedBy)
                .Where(x => x.DishId == itemId || x.DrinkId == itemId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .Select(x => new CommentViewModel()
                {
                    AuthorName = x.CommentedBy.FirstName + " " + x.CommentedBy.LastName,
                    CommentedById = x.CommentedById,
                    CommentText = x.CommentText,
                    DislikesCount = x.Dislikes.Count,
                    LikesCount = x.Likes.Count,
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                })
                .ToList();
            foreach (var comment in actual)
            {
                if (comment.CommentedById == userId)
                {
                    comment.IsCommenter = true;
                }
            }

            for (int i = 0; i < expected.Count; i++)
            {
                actual[i].ShouldDeepEqual(expected[i]);
            }
        }

        [Fact]
        public async Task AddCommentAsyncWorksCorrectly()
        {
            await this.PopulateIngredientsAndTypes();
            await this.PopulateUsers();
            await this.PopulateDishes();
            await this.PopulateDrinks();

            var commentDish = new Comment()
            {
                CommentText = "commenthehe",
                CommentedById = "user1",
                DishId = "test1",
                Rating = 3,
            };

            var commentDrink = new Comment()
            {
                CommentText = "drink comment lol",
                CommentedById = "user2",
                DrinkId = "test3",
                Rating = 5,
            };

            await this.CommentService.AddCommentAsync(commentDish.CommentText, commentDish.Rating, commentDish.CommentedById, FoodType.Dish, commentDish.DishId);
            await this.CommentService.AddCommentAsync(commentDrink.CommentText, commentDrink.Rating, commentDrink.CommentedById, FoodType.Drink, commentDrink.DrinkId);

            var actualDish = this.DbContext.Comments.FirstOrDefault(x => x.DishId != null);
            var actualDrink = this.DbContext.Comments.FirstOrDefault(x => x.DrinkId != null);

            actualDish.WithDeepEqual(commentDish)
                .IgnoreSourceProperty(x => x.Id)
                .IgnoreSourceProperty(x => x.CreatedOn)
                .IgnoreSourceProperty(x => x.Dish)
                .IgnoreSourceProperty(x => x.CommentedBy);
            actualDrink.WithDeepEqual(commentDrink)
                .IgnoreSourceProperty(x => x.Id)
                .IgnoreSourceProperty(x => x.CreatedOn)
                .IgnoreSourceProperty(x => x.Dish)
                .IgnoreSourceProperty(x => x.CommentedBy);
        }

        [Fact]
        public async Task DeleteCommentByIdAsyncWorksCorrectlyWhenGivenTheCommentCreator()
        {
            await this.PopulateDB();

            var commentToDelete = this.DbContext.Comments.FirstOrDefault();
            await this.CommentService.DeleteCommentByIdAsync(commentToDelete.Id, commentToDelete.CommentedById);

            var expected = this.DbContext.Comments.FirstOrDefault(x => x.Id == commentToDelete.Id);

            Assert.Null(expected);
        }

        [Fact]
        public async Task DeleteCommentByIdAsyncWorksCorrectlyWhenGivenAnAdmin()
        {
            await this.PopulateDB();

            var rolemanager = this.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var adminRole = new ApplicationRole()
            {
                Name = GlobalConstants.AdministratorRoleName,
            };

            await rolemanager.CreateAsync(adminRole);

            var userId = "user1";

            this.DbContext.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = userId,
                RoleId = adminRole.Id,
            });

            await this.DbContext.SaveChangesAsync();

            var commentToDelete = this.DbContext.Comments.FirstOrDefault();
            await this.CommentService.DeleteCommentByIdAsync(commentToDelete.Id, userId);

            var expected = this.DbContext.Comments.FirstOrDefault(x => x.Id == commentToDelete.Id);

            Assert.Null(expected);
        }

        [Fact]
        public async Task DeleteCommentByIdAsyncWorksThrowsWhenGivenNotOwnerOrAdminUser()
        {
            await this.PopulateDB();
            var userId = "user1";

            var commentToDelete = this.DbContext.Comments.FirstOrDefault();

            await Assert.ThrowsAsync<Exception>(async () => await this.CommentService.DeleteCommentByIdAsync(commentToDelete.Id, userId));
        }

        [Fact]
        public async Task GetCommentsCountForItemWorksCorrectly()
        {
            await this.PopulateDB();
            var dishId = "test1";
            var drinkId = "test3";

            var actualDish = this.CommentService.GetCommentsCountForItem(dishId);
            var actualDrink = this.CommentService.GetCommentsCountForItem(drinkId);
            var expectedDish = this.DbContext.Comments.Where(x => x.DishId == dishId).Count();
            var expectedDrink = this.DbContext.Comments.Where(x => x.DrinkId == drinkId).Count();

            Assert.Equal(expectedDish, actualDish);
            Assert.Equal(expectedDrink, actualDrink);
        }

        private async Task PopulateDB()
        {
            await this.PopulateIngredientsAndTypes();
            await this.PopulateUsers();
            await this.PopulateDishes();
            await this.PopulateDrinks();
            await this.PopulateComments();
        }

        private async Task PopulateIngredientsAndTypes()
        {
            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test1",
                Id = 1,
            });

            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test2",
                Id = 2,
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 2,
                Name = "test2",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 1,
                Name = "test1",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task PopulateUsers()
        {
            var role1 = new ApplicationRole()
            {
                Id = "role1",
                Name = "role1",
            };
            var role2 = new ApplicationRole()
            {
                Id = "role2",
                Name = "role2",
            };
            var role3 = new ApplicationRole()
            {
                Id = "role3",
                Name = "role3",
            };

            await this.DbContext.Roles.AddAsync(role1);
            await this.DbContext.Roles.AddAsync(role2);
            await this.DbContext.Roles.AddAsync(role3);
            await this.DbContext.SaveChangesAsync();

            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user1",
                FirstName = "first1",
                LastName = "last1",
                Email = "first@aaa.bg",
                PhoneNumber = "111111",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user1", RoleId = "role1", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user2",
                FirstName = "first2",
                LastName = "last2",
                Email = "second@aaa.bg",
                PhoneNumber = "222222",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user2", RoleId = "role2", }, new IdentityUserRole<string>() { UserId = "user2", RoleId = "role3", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user3",
                FirstName = "first3",
                LastName = "last3",
                Email = "third@aaa.bg",
                PhoneNumber = "333333",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user3", RoleId = "role3", } },
            });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task PopulateDishes()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test1", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test2", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task PopulateDrinks()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test3",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test5", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 1,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test4",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test6", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 2,
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task PopulateComments()
        {
            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 1,
                    CommentText = "test",
                    Rating = 3,
                    CommentedById = "user2",
                    DishId = "test1",
                });

            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 2,
                    CommentText = "test2",
                    Rating = 3,
                    CommentedById = "user2",
                    DishId = "test1",
                });

            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 3,
                    CommentText = "test2",
                    Rating = 3,
                    CommentedById = "user2",
                    DrinkId = "test3",
                });

            await this.DbContext.SaveChangesAsync();
        }
    }
}
