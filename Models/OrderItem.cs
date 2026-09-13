using Microsoft.EntityFrameworkCore;

namespace Bookstore.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public Order Order { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
