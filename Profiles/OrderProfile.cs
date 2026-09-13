using AutoMapper;
using Bookstore.DTOs.Orders;
using Bookstore.DTOs.Payments;
using Bookstore.Models;

namespace Bookstore.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Book.Title));

            CreateMap<Payment, PaymentSummaryDto>();

            CreateMap<Order, OrderReadDto>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
                .ForMember(d => d.Payment, o => o.MapFrom(s => s.Payment))
                .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User != null ? s.User.Email : null));
        }
    }
}
