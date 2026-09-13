using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Bookstore.Data;
using Bookstore.Models;
using Bookstore.DTOs.Books;
using Microsoft.AspNetCore.Authorization;
using Bookstore.Helpers;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Przykład: GET /api/books?page=1&pageSize=12&categoryId=2
        [HttpGet]
        public ActionResult<PagedResult<BookReadDto>> GetBooks(
            [FromQuery] int? categoryId,
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Books
                .Include(b => b.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            // wyszukiwanie po tytule lub autorze (bez rozróżnienia wielkości liter)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(term) ||
                    b.Author.ToLower().Contains(term));
            }

            // sortowanie, domyślnie po BookId
            query = sort switch
            {
                "title" => query.OrderBy(b => b.Title),
                "price_asc" => query.OrderBy(b => b.Price),
                "price_desc" => query.OrderByDescending(b => b.Price),
                _ => query.OrderBy(b => b.BookId)
            };

            var totalCount = query.Count();
            var books = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dto = _mapper.Map<IEnumerable<BookReadDto>>(books);
            var result = new PagedResult<BookReadDto>
            {
                Items = dto,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }
        // GET: api/books/id
        [HttpGet("{id}")]
        public ActionResult<BookReadDto> GetBook(int id)
        {
            var book = _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
                return NotFound();

            var dto = _mapper.Map<BookReadDto>(book);
            return Ok(dto);
        }

        // POST: api/books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<BookReadDto> CreateBook(BookCreateDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);

            _context.Books.Add(book);
            _context.SaveChanges();

            // ponowne pobranie z Category, żeby CategoryName nie było null
            var createdBook = _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == book.BookId);

            var readDto = _mapper.Map<BookReadDto>(createdBook);

            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, readDto);
        }

        // PUT: api/books/id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateBook(int id, BookUpdateDto bookDto)
        {
            var existing = _context.Books.Find(id);
            if (existing == null)
                return NotFound();

            _mapper.Map(bookDto, existing);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/books/id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
                return NotFound();

            var isBookOrdered = _context.OrderItems.Any(oi => oi.BookId == id);
            if (isBookOrdered)
                return BadRequest(new
                {
                    message = "Nie można usunąć książki, ponieważ występuje ona w złożonych zamówieniach."
                });

            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
