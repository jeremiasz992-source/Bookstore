namespace Bookstore.DTOs.Payments
{
    public class PaymentSummaryDto
    {
        public int PaymentId { get; set; }
        public string Status { get; set; } = "Pending";
        public string Method { get; set; } = string.Empty;
    }
}
