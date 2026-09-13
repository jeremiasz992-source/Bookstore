using System.Collections.Generic;
using Bookstore.DTOs.Orders;

namespace Bookstore.Services.Interfaces
{
    public interface IOrderService
    {
        OrderReadDto Checkout(int userId);
        IEnumerable<OrderReadDto> GetMyOrders(int userId);
        IEnumerable<OrderReadDto> GetAllOrders();
        OrderReadDto UpdateStatus(int orderId, string status);
    }
}
