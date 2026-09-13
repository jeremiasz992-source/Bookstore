namespace Bookstore.DTOs.Payments
{
    public class PaymentReadDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
