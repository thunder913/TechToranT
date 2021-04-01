namespace RestaurantMenuProject.Services.Messaging
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(
            string from,
            string fromName,
            string to,
            string subject,
            string htmlContent,
            IEnumerable<EmailAttachment> attachments = null);

        public Task SendMakeOrderEmailAsync(string from, string fromName, string to, string receiverName);
    }

}
