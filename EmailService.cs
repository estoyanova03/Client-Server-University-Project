using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Cms;
using WebApplication1.Models;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace WebApplication1.Services
{
    public class EmailService
    {
        private readonly cvContext _context;

        public EmailService(cvContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress("emi_dox0908@abv.bg");
                message.To.Add(new MailAddress(recipientEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient("smtp-relay.brevo.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("841b98001@smtp-brevo.com", "D2KcNL8gVpZUIB0S");
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await client.SendMailAsync(message);
                }
            }
        }

    }
}
