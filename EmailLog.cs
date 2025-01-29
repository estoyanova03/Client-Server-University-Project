using WebApplication1.Services;

namespace WebApplication1.Models
{
    public class EmailLog
    {
        public Guid EntryId { get; set; }
        public string Sender { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsSuccess { get; set; }

        public static implicit operator EmailLog(EmailService v)
        {
            throw new NotImplementedException();
        }
    }
}
