namespace RestaurantMenuProject.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using RestaurantMenuProject.Data.Models;

    public interface IDishService
    {
        public Task AddDish(Dish dish);

        public void RemoveDish(Dish dish);

        public Dish GetById(int id);
    }
}
