namespace RestaurantMenuProject.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RestaurantMenuProject.Services.Data;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Web.ViewModels;
    using RestaurantMenuProject.Web.ViewModels.Administration.Dashboard;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;
        private readonly IUserService userService;

        public DashboardController(ISettingsService settingsService, IUserService userService)
        {
            this.settingsService = settingsService;
            this.userService = userService;
        }

        public IActionResult Index(int id = 1)
        {
            const int itemsPerPage = 12;
            var viewModel = new UserDetailsViewModel()
            {
                Page = id,
                Users = this.userService.GetAllUserDetails(itemsPerPage, id),
                UsersPerPage = itemsPerPage,
                UsersCount = this.userService.GetUsersCount(),
            };

            return this.View(viewModel);
        }
    }
}
