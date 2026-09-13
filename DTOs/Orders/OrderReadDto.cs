using System;
using System.Collections.Generic;
using Bookstore.DTOs.Payments;

namespace Bookstore.DTOs.Orders
{
    public class OrderReadDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public string? UserEmail { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public PaymentSummaryDto? Payment { get; set; }
        public bool ConfirmationEmailSent { get; set; }
        public string? ConfirmationMessage { get; set; }
    }

    public class OrderItemDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
