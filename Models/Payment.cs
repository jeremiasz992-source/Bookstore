using Microsoft.EntityFrameworkCore;

namespace Bookstore.Models
{
    public enum PaymentStatus { Pending = 0, Paid = 1 }

    public enum PaymentMethod { Card = 1, Blik = 2, Transfer = 3 }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProviderTransactionId { get; set; }

        public Order Order { get; set; } = null!;
    }
}
