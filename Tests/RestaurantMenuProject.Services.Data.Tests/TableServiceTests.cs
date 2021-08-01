using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantMenuProject.Services.Data.Tests
{
    public class TableServiceTests : BaseServiceTests
    {
        private ITableService TableService => this.ServiceProvider.GetRequiredService<ITableService>();

        [Fact]
        public async Task GetTableIdByCodeWorksCorrectly()
        {
            await this.PopulateDB();

            var expected = 1;
            var actual = this.TableService.GetTableIdByCode("code1");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetTableByIdCodeReturn0WhenGivenInvalidCode()
        {
            var expected = 0;
            var actual = this.TableService.GetTableIdByCode("INVALID!");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAllTablesWorksCorrectly()
        {
            await this.PopulateDB();

            var expected = this.DbContext.Tables
                    .Select(x => new TableDisplayViewModel()
                    {
                        Capacity = x.Capacity,
                        Code = x.Code,
                        DateCreated = x.CreatedOn,
                        Id = x.Id,
                        Number = x.Number,
                        DateGenerated = x.ModifiedOn,
                    })
                    .ToList();
            var actual = this.TableService.GetAllTables();

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public async Task AddTableAsyncWorksCorrectly()
        {
            var addTable = new AddTableViewModel()
            {
                Id = 15,
                Number = 5,
                Capacity = 6,
            };

            await this.TableService.AddTableAsync(addTable);
            var actual = this.DbContext.Tables.FirstOrDefault(x => x.Id == 15);

            Assert.Equal(15, actual.Id);
            Assert.Equal(5, actual.Number);
            Assert.Equal(6, actual.Capacity);
        }

        [Fact]
        public async Task AddTableAsyncThrowsExceptionWhenGivenExistingTableNumber()
        {
            await this.PopulateDB();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.TableService.AddTableAsync(new AddTableViewModel() { Number = 1 }));
        }

        [Fact]
        public async Task GetTableByIdWorksCorrectly()
        {
            await this.PopulateDB();
            var id = 1;

            var expected = this.DbContext.Tables.To<AddTableViewModel>().FirstOrDefault(x => x.Id == id);
            var actual = this.TableService.GetTableById(id);

            actual.IsDeepEqual(expected);
        }

        [Fact]
        public void GetTableByIdThrowsExceptionWhenGivenInvalidId()
        {
            Assert.Throws<InvalidOperationException>(() => this.TableService.GetTableById(99991));
        }

        [Fact]
        public async Task RemoveTableAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var id = 2;

            await this.TableService.RemoveTableAsync(id);
            var actual = this.DbContext.Tables.FirstOrDefault(x => x.Id == id);

            Assert.Null(actual);
        }

        [Fact]
        public async Task RemoveTableAsyncThrowsExceptionWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.TableService.RemoveTableAsync(991321));
        }

        [Fact]
        public async Task EditTableAsyncWorksCorrectly()
        {
            await this.PopulateDB();
            var editTable = new AddTableViewModel()
            {
                Id = 1,
                Capacity = 20,
                Number = 19,
            };

            await this.TableService.EditTableAsync(editTable);
            var actual = this.DbContext.Tables.FirstOrDefault(x => x.Id == 1);

            Assert.Equal(editTable.Id, actual.Id);
            Assert.Equal(editTable.Capacity, actual.Capacity);
            Assert.Equal(editTable.Number, actual.Number);
            Assert.NotEqual("code1", actual.Code);
        }

        [Fact]
        public async Task EditTableAsyncThrowsExceptionWhenGivenExistingTableNumber()
        {
            await this.PopulateDB();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await this.TableService.EditTableAsync(new AddTableViewModel() { Number = 1 }));
        }

        [Fact]
        public async Task EditTableAsyncThrowsWhenGivenInvalidId()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.TableService.EditTableAsync(new AddTableViewModel() { Id = 9123 }));
        }

        [Fact]
        public async Task RefreshTableCodesAsyncWorksCorrectly()
        {
            await this.PopulateDB();

            var oldCodes = this.DbContext.Tables.Select(x => x.Code).ToList();
            await this.TableService.RefreshTableCodesAsync();
            var newCode = this.DbContext.Tables.Select(x => x.Code).ToList();

            for (int i = 0; i < oldCodes.Count; i++)
            {
                Assert.NotEqual(oldCodes[i], newCode[i]);
            }
        }

        [Fact]
        public async Task IsTableCodeFreeWorksCorrectly()
        {
            await this.PopulateDB();

            var falseTable = this.TableService.IsTableCodeFree("code1");
            var trueTable = this.TableService.IsTableCodeFree("ASDHg");

            Assert.False(falseTable);
            Assert.True(trueTable);
        }

        [Fact]
        public async Task GetFreeTableWorksCorrectlyAtTheEnd()
        {
            await this.PopulateDB();

            var expected = 4;
            var actual = this.TableService.GetFreeTable();

            Assert.Equal(actual, expected);
        }

        [Fact]
        public async Task GetFreeTableWorksCorrectlyInTheMiddle()
        {
            await this.PopulateDB();
            var toRemove = this.DbContext.Tables.FirstOrDefault(x => x.Id == 2);
            this.DbContext.Tables.Remove(toRemove);
            await this.DbContext.SaveChangesAsync();

            var expected = 2;
            var actual = this.TableService.GetFreeTable();

            Assert.Equal(actual, expected);
        }

        private async Task PopulateDB()
        {
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
        }
    }
}
