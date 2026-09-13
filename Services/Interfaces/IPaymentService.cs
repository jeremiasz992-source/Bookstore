using Bookstore.DTOs.Payments;

namespace Bookstore.Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentReadDto Create(int orderId, string method, int userId);
        PaymentReadDto MarkPaid(int orderId, string providerTxnId, int userId);
    }
}
