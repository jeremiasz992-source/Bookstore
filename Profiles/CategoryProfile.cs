using AutoMapper;
using Bookstore.Models;
using Bookstore.DTOs.Categories;

namespace Bookstore.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // Mapowania dla Category
            CreateMap<Category, CategoryReadDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}
