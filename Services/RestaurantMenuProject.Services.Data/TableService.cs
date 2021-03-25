using RestaurantMenuProject.Data.Common.Repositories;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Data.Contracts;
using RestaurantMenuProject.Services.Mapping;
using RestaurantMenuProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void AddTable(AddTableViewModel tableViewModel)
        {
            var mapper = AutoMapperConfig.MapperInstance;
            if (this.tableRepository.All().Any(x => x.Number == tableViewModel.Number))
            {
                // TODO add it to the middlewares
                throw new InvalidOperationException("The table number already exists!");
            }

            var table = mapper.Map<AddTableViewModel, Table>(tableViewModel);
            table.Code = this.RandomString(6);
            this.tableRepository.AddAsync(table).GetAwaiter().GetResult();
            this.tableRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public AddTableViewModel GetTableById(int id)
        {
            return this.tableRepository.All().To<AddTableViewModel>().First(x => x.Id == id);
        }

        public void RemoveTable(int id)
        {
            var tableToDelete = this.tableRepository.AllAsNoTracking().First(x => x.Id == id);
            this.tableRepository.Delete(tableToDelete);
            this.tableRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public void EditTable(AddTableViewModel tableViewModel)
        {
            var table = this.tableRepository.All().FirstOrDefault(x => x.Id == tableViewModel.Id);

            table.Number = tableViewModel.Number;
            table.Capacity = tableViewModel.Capacity;
            table.Code = this.RandomString(6);
            this.tableRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public void RefreshTableCodes()
        {
            var tables = this.tableRepository.All();
            foreach (var table in tables)
            {
                table.Code = this.RandomString(6);
            }
            this.tableRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private string RandomString(int length)
        {
            Random random = new Random();

        // TODO use some other generation method
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string (Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
