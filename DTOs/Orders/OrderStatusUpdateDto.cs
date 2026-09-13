namespace Bookstore.DTOs.Orders
{
    // Dane przesyłane przez administratora przy zmianie stanu zamówienia.
    public class OrderStatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
