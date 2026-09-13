using Bookstore.Models;

namespace Bookstore.Services.Interfaces
{
    public interface IEmailService
    {
        void SendOrderConfirmation(Order order, string recipientEmail);
        void SendPaymentConfirmation(Order order, string recipientEmail);
    }
}
