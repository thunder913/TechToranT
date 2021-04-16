using DeepEqual.Syntax;
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

        [Fact]
        public async Task GetAllItemsToPickUpWorksCorrectly()
        {
            await this.PopulateDB();
            var waiterId = "test1";
            var actual = this.PickupItemService.GetAllItemsToPickUp(waiterId);
            var expected = this.DbContext.PickupItems.Where(x => x.WaiterId == waiterId).ToList();

            actual.IsDeepEqual(expected);
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

            actual.IsDeepEqual(expected);
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

        private async Task PopulateDB()
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
        }
    }
}
