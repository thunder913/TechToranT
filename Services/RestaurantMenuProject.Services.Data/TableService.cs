using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data
{
    public class TableService : ITableService
    {
        private readonly IDeletableEntityRepository<Table> tableRepository;

        public TableService(
            IDeletableEntityRepository<Table> tableRepository)
        {
            this.tableRepository = tableRepository;
        }

        public int GetTableIdByCode(string code)
        {
            var table = this.tableRepository
                    .AllAsNoTracking()
                    .FirstOrDefault(x => x.Code == code);

            if (table != null)
            {
                return table.Id;
            }

            return 0;
        }

        public ICollection<TableDisplayViewModel> GetAllTables()
        {
            return this.tableRepository
                .All()
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
        }

        public async Task AddTableAsync(AddTableViewModel tableViewModel)
        {
            var mapper = AutoMapperConfig.MapperInstance;
            if (this.tableRepository.All().Any(x => x.Number == tableViewModel.Number))
            {
                throw new InvalidOperationException("The table number already exists!");
            }

            var table = mapper.Map<AddTableViewModel, Table>(tableViewModel);
            table.Code = this.RandomString(6);
            await this.tableRepository.AddAsync(table);
            await this.tableRepository.SaveChangesAsync();
        }

        public AddTableViewModel GetTableById(int id)
        {
            return this.tableRepository.All().To<AddTableViewModel>().First(x => x.Id == id);
        }

        public async Task RemoveTableAsync(int id)
        {
            var tableToDelete = this.tableRepository.All().First(x => x.Id == id);
            this.tableRepository.Delete(tableToDelete);
            await this.tableRepository.SaveChangesAsync();
        }

        public async Task EditTableAsync(AddTableViewModel tableViewModel)
        {
            var table = this.tableRepository.All().FirstOrDefault(x => x.Id == tableViewModel.Id);

            table.Number = tableViewModel.Number;
            table.Capacity = tableViewModel.Capacity;
            table.Code = this.RandomString(6);
            await this.tableRepository.SaveChangesAsync();
        }

        public async Task RefreshTableCodesAsync()
        {
            var tables = this.tableRepository.All();
            foreach (var table in tables)
            {
                table.Code = this.RandomString(6);
            }
            await this.tableRepository.SaveChangesAsync();
        }

        public bool IsTableCodeFree(string code)
        {
            if (this.tableRepository.All().Any(x => x.Code == code))
            {
                return false;
            }

            return true;
        }

        public int GetFreeTable()
        {
            var tables = this.tableRepository.All().OrderByDescending(x => x.Number).ToArray();
            for (int i = 1; i < tables[0].Number; i++)
            {
                if (!tables.Any(x => x.Number == i))
                {
                    return i;
                }
            }

            return tables[0].Number + 1;
        }

        private string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomCode = new string (Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            if (!this.IsTableCodeFree(randomCode))
            {
                randomCode = this.RandomString(length);
            }

            return randomCode;
        }
    }
}
