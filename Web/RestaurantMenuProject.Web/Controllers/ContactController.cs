using Microsoft.AspNetCore.Mvc;
using RestaurantMenuProject.Common;
using RestaurantMenuProject.Services.Messaging;
using RestaurantMenuProject.Web.ViewModels;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender emailSender;

        public ContactController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Submit(ContactViewModel contactViewModel)
        {
            await this.emailSender.SendEmailAsync(GlobalConstants.Email, contactViewModel.Name, "andon_00@abv.bg", "Feedback", contactViewModel.Message + $"\r\n Sent from {contactViewModel.Email} - {contactViewModel.Phone}");
            await this.emailSender.SendEmailAsync(GlobalConstants.Email, GlobalConstants.SystemName, contactViewModel.Email, "We got you!", "We received your feedback! Thank you for writing to us! TechToranT!");
            return this.RedirectToAction("Index", "Home");
        }
    }
}
