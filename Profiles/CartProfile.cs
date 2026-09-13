using AutoMapper;
using Bookstore.Models;
using Bookstore.DTOs.Cart;

namespace Bookstore.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartReadDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Book.Price));

            CreateMap<CartItemCreateDto, CartItem>();
        }
    }
}
