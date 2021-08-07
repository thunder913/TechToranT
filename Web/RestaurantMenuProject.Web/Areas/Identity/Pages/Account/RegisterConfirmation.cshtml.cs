using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RestaurantMenuProject.Data.Models;
using RestaurantMenuProject.Services.Messaging;
using RestaurantMenuProject.Common;
using System.Text.Encodings.Web;

namespace RestaurantMenuProject.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _sender = sender;
            this._signInManager = signInManager;
        }

        public string Email { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
            protocol: Request.Scheme);

            await _sender.SendEmailAsync(GlobalConstants.Email, GlobalConstants.SystemName, email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Page();
        }
    }
}
