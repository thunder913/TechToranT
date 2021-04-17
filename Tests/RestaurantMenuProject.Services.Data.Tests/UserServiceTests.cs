using DeepEqual.Syntax;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class UserServiceTests : BaseServiceTests
    {
        private IUserService UserService => this.ServiceProvider.GetRequiredService<IUserService>();

        [Fact]
        public async Task GetUserByIdWorksCorrectly()
        {
            await this.PopulateDB();
            var id = "user2";

            var expected = this.DbContext.Users.FirstOrDefault(x => x.Id == id);
            var actual = this.UserService.GetUserById(id);

            actual.IsDeepEqual(expected);
        }

        [Theory]
        [InlineData("Name", "desc", "a")]
        [InlineData("Name", "desc", "2")]
        [InlineData("Name", "desc", ":")]
        [InlineData("Name", "desc", "/")]
        [InlineData("Name", "desc", "role2")]
        [InlineData("Name", "desc", "first1")]
        [InlineData("Name", "desc", "last2")]
        [InlineData("Name", "desc", "@")]
        [InlineData(" ", "desc", "@")]
        [InlineData("Name", "", "@")]
        public async Task GetUserDataAsQueryableWorksCorrectly(string sortColumn, string sortDirection, string searchValue)
        {
            await this.PopulateDB();

            var users = this.DbContext.Users
                .To<UserWithRolesViewModel>()
                .Select(x => new UserViewModel()
                {
                    CreatedOn = x.CreatedOn,
                    DeletedOn = x.DeletedOn,
                    Email = x.Email,
                    Id = x.Id,
                    Name = x.Name,
                    Roles = string.Join(", ", this.DbContext.Roles.Where(y => x.RoleIds.Contains(y.Id))),
                });

            if (!(string.IsNullOrWhiteSpace(sortColumn) || string.IsNullOrWhiteSpace(sortDirection)))
            {
                users = users.OrderBy(sortColumn + " " + sortDirection);
            }

            var expected = users.ToList();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                expected = expected.Where(m => m.Email.ToLower().Contains(searchValue.ToLower())
                                        || m.Name.ToLower().Contains(searchValue.ToLower())
                                        || m.Roles.ToLower().Contains(searchValue.ToLower())
                                        || m.CreatedOn.ToLocalTime().ToString("dd/MM/yyyy, HH:mm:ss").Contains(searchValue)
                                        || m.DeletedOn.ToString().Contains(searchValue)).ToList();

            }

            var actual = this.UserService.GetUserDataAsQueryable(sortColumn, sortDirection, searchValue);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task EditUserDataAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var newRole = this.DbContext.Roles.FirstOrDefault(x => x.Id == "role1");
            var edituser = new EditUserViewModel()
            {
                Id = "user1",
                Email = "newmail@abv.bg",
                FirstName = "newName",
                LastName = "newLASTname",
                PhoneNumber = "9922338811",
                RoleIds = new List<string>() { "role1", },
            };

            await this.UserService.EditUserDataAsync(edituser);
            var actual = this.DbContext.Users.FirstOrDefault(x => x.Id == "user1");

            Assert.Equal(edituser.Email, actual.Email);
            Assert.Equal(edituser.FirstName, actual.FirstName);
            Assert.Equal(edituser.LastName, actual.LastName);
            Assert.Equal(edituser.PhoneNumber, actual.PhoneNumber);
            Assert.Equal(1, actual.Roles.Count);
            actual.Roles.FirstOrDefault().IsDeepEqual(newRole);
        }

        [Fact]
        public async Task EditUserDataAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.UserService.EditUserDataAsync(new EditUserViewModel() { Id = "INVALID", FirstName = "test" }));
        }

        [Fact]
        public async Task SetUserNamesAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var id = "user3";
            var firstName = "test1332";
            var lastName = "test99331";

            await this.UserService.SetUserNamesAsync(id, firstName, lastName);
            var actual = this.DbContext.Users.FirstOrDefault(x => x.Id == id);

            Assert.Equal(firstName, actual.FirstName);
            Assert.Equal(lastName, actual.LastName);
        }

        [Fact]
        public async Task SetUserNamesAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.UserService.SetUserNamesAsync("INVALID", "test", "test31"));
        }

        [Fact]
        public async Task DeleteUserAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var expectedCount = this.DbContext.Users.Count() - 1;

            var user = this.DbContext.Users.Skip(2).FirstOrDefault();
            await this.UserService.DeleteUserAsync(user);

            Assert.Equal(expectedCount, this.DbContext.Users.Count());
        }

        [Fact]
        public async Task GetUsersCountWorksCorrectly()
        {
            await this.PopulateDB();

            var expectedCount = this.DbContext.Users.Count();
            var actualCount = this.UserService.GetUsersCount();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetUserRolesWorksCorrectly()
        {
            await this.PopulateDB();

            var expectedRoles = this.DbContext.Roles;
            var actualRoles = this.UserService.GetUserRoles();

            actualRoles.IsDeepEqual(expectedRoles);
        }

        private async Task PopulateDB()
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
    }
}
