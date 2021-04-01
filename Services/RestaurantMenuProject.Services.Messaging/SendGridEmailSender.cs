namespace RestaurantMenuProject.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class SendGridEmailSender : IEmailSender
    {
        private const string NewOrdeTemplate = "d-de9afe6e40c84d1b979384b12774c59e";
        private readonly SendGridClient client;

        public SendGridEmailSender(string apiKey)
        {
            this.client = new SendGridClient(apiKey);
        }

        public async Task SendEmailAsync(string from, string fromName, string to, string subject, string htmlContent, IEnumerable<EmailAttachment> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("Subject and message should be provided.");
            }

            var message = this.GetMessageWithData(from, to, fromName, subject, htmlContent);

            if (attachments?.Any() == true)
            {
                foreach (var attachment in attachments)
                {
                    message.AddAttachment(attachment.FileName, Convert.ToBase64String(attachment.Content), attachment.MimeType);
                }
            }

            await this.client.SendEmailAsync(message);
        }

        public async Task SendMakeOrderEmailAsync(string from, string fromName, string to, string receiverName)
        {
            var message = this.GetMessageWithData(from, to, fromName, "You successfully sent your order!");
            message.SetTemplateId(NewOrdeTemplate);
            message.SetTemplateData(new
            {
                receiver_name = receiverName,
            });

            var sentMessage = await this.client.SendEmailAsync(message);
        }


        private SendGridMessage GetMessageWithData(string from, string to, string fromName, string subject, string htmlContent = null)
        {
            var fromAddress = new EmailAddress(from, fromName);
            var toAddress = new EmailAddress(to);
            var message = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, null, htmlContent);
            return message;
        }
    }
}
