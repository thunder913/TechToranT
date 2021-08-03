using DeepEqual.Syntax;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Data.Models.Enums;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class PickupItemServiceTests : BaseServiceTests
    {
        private IPickupItemService PickupItemService => this.ServiceProvider.GetRequiredService<IPickupItemService>();

        private IOrderDishService OrderDishService => this.ServiceProvider.GetRequiredService<IOrderDishService>();

        [Fact]
        public async Task GetAllItemsToPickUpWorksCorrectly()
        {
            await this.PopulateDB();
            var waiterId = "test1";
            var actual = this.PickupItemService.GetAllItemsToPickUp(waiterId);
            var expected = this.DbContext.PickupItems.Where(x => x.WaiterId == waiterId).ToList();

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public async Task DeleteItemsAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var id = this.DbContext.PickupItems.Skip(1).FirstOrDefault().Id;

            await this.PickupItemService.DeleteItemAsync(id);
            var actual = this.DbContext.PickupItems.FirstOrDefault(x => x.Id == id);

            Assert.Null(actual);
        }

        [Fact]
        public async Task DeleteItemsAsyncThrowsExceptionWhenGivenInvalidUserId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.PickupItemService.DeleteItemAsync("INVALID!"));
        }

        [Fact]
        public async Task GetPickupItemByIdWorksCorrectly()
        {
            await this.PopulateDB();

            var expected = this.DbContext.PickupItems.Skip(2).FirstOrDefault();
            var actual = this.PickupItemService.GetPickupItemById(expected.Id);

            actual.ShouldDeepEqual(expected);
        }

        [Fact]
        public void GetPickupItemByIdReturnsNull()
        {
            Assert.Null(this.PickupItemService.GetPickupItemById("invalid!"));
        }

        [Fact]
        public async Task IsOrderFullyDeliveredReturnsFalseWhenOrderIsNotFinished()
        {
            await this.PopulateDB();

            var orderId = this.DbContext.PickupItems.FirstOrDefault().OrderId;
            var isDelivered = this.PickupItemService.IsOrderFullyDelivered(orderId);

            Assert.False(isDelivered);
        }

        [Fact]
        public async Task AddPickupItemAsyncWithDishWorksCorrectly()
        {
            await this.PopulateDB();

            var orderDish = this.DbContext.OrderDishes.FirstOrDefault();

            var model = new CookFinishItemViewModel()
            {
                DishType = FoodType.Dish,
                OrderId = orderDish.OrderId,
                FoodId = orderDish.DishId,
            };
            var oldPickupItem = this.OrderDishService.GetOrderDishAsPickupItem(model.FoodId, model.OrderId);

            var actual = await this.PickupItemService.AddPickupItemAsync(model);
            var expected = this.DbContext.PickupItems
                .FirstOrDefault(x => x.TableNumber == oldPickupItem.TableNumber
                && x.OrderId == oldPickupItem.OrderId
                && x.ClientName == oldPickupItem.ClientName
                && x.Name == oldPickupItem.Name);

            actual.ShouldDeepEqual(expected.Id);
        }

        [Fact]
        public async Task AddPickupItemAsyncWithDrinkWorksCorrectly()
        {
            await this.PopulateDB();

            var orderDish = this.DbContext.OrderDrinks.FirstOrDefault();

            var model = new CookFinishItemViewModel()
            {
                DishType = FoodType.Drink,
                OrderId = orderDish.OrderId,
                FoodId = orderDish.DrinkId,
            };
            var oldPickupItem = this.OrderDishService.GetOrderDishAsPickupItem(model.FoodId, model.OrderId);

            var actual = await this.PickupItemService.AddPickupItemAsync(model);
            var expected = this.DbContext.PickupItems
                .FirstOrDefault(x => x.TableNumber == oldPickupItem.TableNumber
                && x.OrderId == oldPickupItem.OrderId
                && x.ClientName == oldPickupItem.ClientName
                && x.Name == oldPickupItem.Name);

            actual.ShouldDeepEqual(expected.Id);
        }

        [Fact]
        public async Task AddPickupItemAsyncWhenPickupItemIsNull()
        {
            await this.PopulateDB();

            this.DbContext.PickupItems.Add(new PickupItem()
            {
                ClientName = "first1 last1",
                OrderId = "order2",
                Name = "test1",
                TableNumber = 2,
            });

            await this.DbContext.SaveChangesAsync();

            var orderDish = this.DbContext.OrderDrinks.FirstOrDefault();

            var model = new CookFinishItemViewModel()
            {
                DishType = FoodType.Drink,
                OrderId = orderDish.OrderId,
                FoodId = orderDish.DrinkId,
            };
            var oldPickupItem = this.OrderDishService.GetOrderDishAsPickupItem(model.FoodId, model.OrderId);

            var actual = await this.PickupItemService.AddPickupItemAsync(model);
            var expected = this.DbContext.PickupItems
                .FirstOrDefault(x => x.TableNumber == oldPickupItem.TableNumber
                && x.OrderId == oldPickupItem.OrderId
                && x.ClientName == oldPickupItem.ClientName
                && x.Name == oldPickupItem.Name);

            actual.ShouldDeepEqual(expected.Id);
        }

        private new async Task PopulateDB()
        {
            this.DbContext.PickupItems.Add(new PickupItem()
            {
                Id = "test1",
                Count = 3,
                TableNumber = 2,
                Name = "test1 test1",
                ClientName = "test1 test1",
                WaiterId = "test1",
                OrderId = "test1",
            });

            this.DbContext.PickupItems.Add(new PickupItem()
            {
                Id = "test2",
                Count = 4,
                TableNumber = 2,
                Name = "test2 test2",
                ClientName = "test2 test2",
                WaiterId = "test1",
                OrderId = "test1",
            });

            this.DbContext.PickupItems.Add(new PickupItem()
            {
                Id = "test3",
                Count = 9,
                TableNumber = 3,
                Name = "test3 test3",
                ClientName = "test3 test3",
                WaiterId = "test2",
                OrderId = "test2",
            });

            await this.DbContext.SaveChangesAsync();

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
                    DishTypeId = 2,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

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

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
            {
                OrderId = "order1",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
            {
                OrderId = "order2",
                Count = 3,
                DeliveredCount = 1,
                DishId = "test2",
                PriceForOne = 10,
            });

            await this.DbContext.OrderDishes.AddAsync(new OrderDish()
            {
                OrderId = "order2",
                Count = 5,
                DeliveredCount = 3,
                DishId = "test1",
                PriceForOne = 10,
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

            var dishTypes = this.DbContext.DishTypes.ToList();
            var drinkTypes = this.DbContext.DrinkTypes.ToList();

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 1,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code1",
                PromoPercent = 20,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.PromoCodes.AddAsync(new PromoCode()
            {
                Id = 2,
                ValidDishCategories = dishTypes,
                ValidDrinkCategories = drinkTypes,
                Code = "code2",
                PromoPercent = 10,
                ExpirationDate = DateTime.Now.AddDays(1),
                MaxUsageTimes = 20,
                UsedTimes = 10,
            });

            await this.DbContext.SaveChangesAsync();
        }
    }
}
