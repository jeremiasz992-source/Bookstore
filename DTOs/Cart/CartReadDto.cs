namespace Bookstore.DTOs.Cart
{
    public class CartReadDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
