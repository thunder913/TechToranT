using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Services.Data.Contracts
{
    public interface IPickupItemService
    {
        public ICollection<PickupItem> GetAllItemsToPickUp();

        public Task DeleteItemAsync(string id);

        public Task<bool> AddPickupItemAsync(CookFinishItemViewModel viewModel);
    }
}
