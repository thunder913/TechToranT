using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface ITableService
    {
        public int GetTableIdByCode(string code);

        public ICollection<TableDisplayViewModel> GetAllTables();

        public Task AddTableAsync(AddTableViewModel tableViewModel);

        public AddTableViewModel GetTableById(int id);

        public Task RemoveTableAsync(int id);

        public Task EditTableAsync(AddTableViewModel tableViewModel);

        public Task RefreshTableCodesAsync();

        public bool IsTableCodeFree(string code);

        public int GetFreeTable();
    }
}
