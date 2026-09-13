namespace Bookstore.DTOs.Cart
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
