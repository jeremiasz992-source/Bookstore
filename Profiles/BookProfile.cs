using AutoMapper;
using Bookstore.Models;
using Bookstore.DTOs.Books;

namespace Bookstore.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            // Book -> BookReadDto
            CreateMap<Book, BookReadDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            // BookCreateDto -> Book
            CreateMap<BookCreateDto, Book>();

            // BookUpdateDto -> Book
            CreateMap<BookUpdateDto, Book>();
        }
    }
}
