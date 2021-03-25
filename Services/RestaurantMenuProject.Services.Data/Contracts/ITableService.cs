using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface ITableService
    {
        public int GetTableIdByCode(string code);

        public ICollection<TableDisplayViewModel> GetAllTables();

        public void AddTable(AddTableViewModel tableViewModel);

        public AddTableViewModel GetTableById(int id);

        public void RemoveTable(int id);

        public void EditTable(AddTableViewModel tableViewModel);

        public void RefreshTableCodes();
    }
}
