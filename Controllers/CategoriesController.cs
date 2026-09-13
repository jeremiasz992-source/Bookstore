using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Bookstore.Data;
using Bookstore.Models;
using Bookstore.DTOs.Categories;
using Microsoft.AspNetCore.Authorization;


namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/categories
        [HttpGet]
        public ActionResult<IEnumerable<CategoryReadDto>> GetCategories()
        {
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            var dto = _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
            return Ok(dto);
        }
        // GET: api/categories/id
        [HttpGet("{id}")]
        public ActionResult<CategoryReadDto> GetCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();
            var dto = _mapper.Map<CategoryReadDto>(category);
            return Ok(dto);
        }
        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<CategoryReadDto> CreateCategory(CategoryCreateDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto?.Name))
                return BadRequest(new { message = "Nazwa kategorii nie może być pusta." });

            categoryDto.Name = categoryDto.Name.Trim();
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            _context.SaveChanges();
            var dto = _mapper.Map<CategoryReadDto>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, dto);
        }
        // PUT: api/categories/id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto?.Name))
                return BadRequest(new { message = "Nazwa kategorii nie może być pusta." });

            categoryDto.Name = categoryDto.Name.Trim();
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();
            _mapper.Map(categoryDto, category);
            _context.SaveChanges();
            return NoContent();
        }
        // DELETE: api/categories/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCategory(int id)
        {
            if (id == 1)
            {
                return BadRequest(new
                {
                    message = "Nie można usunąć kategorii domyślnej."
                });
            }

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();
            // Sprawdzenie, czy kategoria jest używana przez jakieś książki
            var isCategoryInUse = _context.Books.Any(b => b.CategoryId == id);
            if (isCategoryInUse)
            {
                return BadRequest(new
                {
                    message = "Nie można usunąć kategorii, ponieważ są do niej przypisane książki."
                });
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
