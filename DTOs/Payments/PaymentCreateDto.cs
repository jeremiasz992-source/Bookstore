namespace Bookstore.DTOs.Payments
{
    public class PaymentCreateDto
    {
        public string? Method { get; set; } // Card/Blik/Transfer
        // Kwota jest obliczona przez serwer na podstawie pozycji zamówienia.
    }
}
