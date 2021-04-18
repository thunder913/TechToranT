using DeepEqual.Syntax;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class OrderDrinkServiceTests : BaseServiceTests
    {
        private IOrderDrinkService DrinkService => this.ServiceProvider.GetRequiredService<IOrderDrinkService>();

        [Fact]
        public async Task AddDeliveredCountToOrderDrinkAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var foodId = "test2";
            var orderId = "order2";
            var toAdd = 2;
            var expectedCount = this.DbContext.OrderDrinks.FirstOrDefault(x => x.OrderId == orderId && x.DrinkId == foodId).DeliveredCount + toAdd;

            await this.DrinkService.AddDeliveredCountToOrderDrinkAsync(orderId, foodId, toAdd);
            var actualCount = this.DbContext.OrderDrinks.FirstOrDefault(x => x.OrderId == orderId && x.DrinkId == foodId).DeliveredCount;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task AddDeliveredCountToOrderDrinkAsyncThrowsWhenItemHasAlreadyBeenMade()
        {
            await this.PopulateDB();

            var orderDrink = this.DbContext.OrderDrinks.FirstOrDefault();
            orderDrink.DeliveredCount = orderDrink.Count;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.DrinkService.AddDeliveredCountToOrderDrinkAsync(orderDrink.OrderId, orderDrink.DrinkId, 1));
        }

        [Fact]
        public async Task GetOrderDrinkAsPickupItemWorksCorrectly()
        {
            await this.PopulateDB();
            var orderDrink = this.DbContext.OrderDrinks.FirstOrDefault();

            var expected = this.DbContext.OrderDrinks
                .Where(x => x.OrderId == orderDrink.OrderId && x.DrinkId == orderDrink.DrinkId)
                .Select(x => new PickupItem()
                {
                    ClientName = x.Order.Client.FirstName + " " + x.Order.Client.LastName,
                    Name = x.Drink.Name,
                    TableNumber = x.Order.Table.Number,
                    WaiterId = x.Order.WaiterId,
                    Count = 1,
                    OrderId = orderDrink.OrderId,
                })
                .FirstOrDefault();
            var actual = this.DrinkService.GetOrderDrinkAsPickupItem(orderDrink.DrinkId, orderDrink.OrderId);

            actual.IsDeepEqual(expected);
        }

        private async Task PopulateDB()
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

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 1,
                Name = "test1",
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

            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test3", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 1,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test4", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 2,
                });


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

            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 1,
                Number = 1,
                Code = "code1",
                Capacity = 4,
            });
            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 2,
                Number = 2,
                Code = "code2",
                Capacity = 6,
            });
            await this.DbContext.Tables.AddAsync(new Table()
            {
                Id = 3,
                Number = 3,
                Code = "code3",
                Capacity = 2,
            });

            await this.DbContext.SaveChangesAsync();

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user3",
                Id = "order1",
                ProcessType = ProcessType.Pending,
                TableId = 1,
                WaiterId = "user2",
            });

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user1",
                Id = "order2",
                ProcessType = ProcessType.Pending,
                TableId = 2,
                WaiterId = "user2",
            });

            await this.DbContext.Orders.AddAsync(new Order()
            {
                ClientId = "user3",
                Id = "order3",
                ProcessType = ProcessType.Pending,
                TableId = 2,
                WaiterId = "user2",
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order1",
                Count = 3,
                DeliveredCount = 1,
                DrinkId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order2",
                Count = 8,
                DeliveredCount = 1,
                DrinkId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDrinks.AddAsync(new OrderDrink()
            {
                OrderId = "order2",
                Count = 5,
                DeliveredCount = 3,
                DrinkId = "test1",
                PriceForOne = 10,
            });

            await this.DbContext.SaveChangesAsync();
        }

    }
}
