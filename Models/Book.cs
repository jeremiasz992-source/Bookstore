using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Author { get; set; } = null!;

        [Required]
        public string Publisher { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        [Range(1450, 2100)]
        public int Year { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        //Domyślna kategoria = 1
        public int CategoryId { get; set; } = 1;
        public Category Category { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
