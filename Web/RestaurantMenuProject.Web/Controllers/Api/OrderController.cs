namespace RestaurantMenuProject.Web.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using RestaurantMenuProject.Common;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Dtos;
    using RestaurantMenuProject.Data.Models.Enums;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Messaging;
    using RestaurantMenuProject.Web.Hubs;

    [Route("api/[Controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IPickupItemService pickupItemService;
        private readonly ITableService tableService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IHubContext<OrderHub> orderHub;

        public OrderController(
            IOrderService orderService,
            IPickupItemService pickupItemService,
            ITableService tableService,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IHubContext<OrderHub> orderHub
            )
        {
            this.orderService = orderService;
            this.pickupItemService = pickupItemService;
            this.tableService = tableService;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.orderHub = orderHub;
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<bool>> Delete(OrderIdDto orderDto)
        {
            return await this.orderService.CancelOrderAsync(orderDto.OrderId);
        }

        [HttpPost("All")]
        public ActionResult GetAllOrders()
        {
            var draw = this.Request.Form["draw"].FirstOrDefault();
            var start = this.Request.Form["start"].FirstOrDefault();
            var length = this.Request.Form["length"].FirstOrDefault();
            var sortColumn = this.Request.Form["columns[" + this.Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = this.Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = this.Request.Form["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var orders = this.orderService.GetAllOrders(sortColumn, sortColumnDirection, searchValue);

            var recordsTotal = orders.Count();
            var data = orders.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

            return this.Ok(jsonData);
        }

        [HttpPost("EditStatus")]
        public async Task<ActionResult<bool>> EditStatusAsync(EditStatusDto editStatus)
        {
            var oldProcessingTypeId = (ProcessType)Enum.Parse(typeof(ProcessType), editStatus.OldProcessingType);
            await this.orderService.ChangeOrderStatusAsync(oldProcessingTypeId, (ProcessType) editStatus.NewProcessingTypeId, editStatus.OrderId);
            return true;
        }

        [HttpPost("AcceptOrder")]
        public async Task<ActionResult<bool>> AcceptOrder(EditStatusDto editStatus)
        {
            var statusEditted = await this.EditStatusAsync(editStatus);
            if (!statusEditted.Value)
            {
                throw new InvalidOperationException("There was an error changing the status of the order!");
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.orderService.AddWaiterToOrderAsync(editStatus.OrderId, userId);

            return true;
        }

        [HttpPost("DonePickup/{id}")]
        public async Task<ActionResult<bool>> DonePickup(string id)
        {
            await this.pickupItemService.DeleteItemAsync(id);
            return true;
        }

        [HttpPost("FinishOrder/{id}")]
        public async Task<ActionResult<bool>> FinishOrderAsync(string id)
        {
            await this.orderService.FinishOrderAsync(id);
            return true;
        }

        [HttpPost("CheckTable/{code?}")]
        public async Task<ActionResult<bool>> CheckTable(string code)
        {
            try
            {
                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await this.userManager.FindByIdAsync(userId);
                var orderId = await this.orderService.MakeOrderAsync(userId, code);
                await this.emailSender.SendMakeOrderEmailAsync(GlobalConstants.Email, "Techtorant", user.Email, user.FirstName + " " + user.LastName);

                var waiterIds = this.userManager.GetUsersInRoleAsync(GlobalConstants.WaiterRoleName).Result.Select(x => x.Id);

                var orderInListItem = this.orderService.GetOrderInListById(orderId);
                await this.orderHub.Clients.Users(waiterIds).SendAsync("AddItemsToPickup", new
                {
                    Order = orderInListItem,
                });
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
