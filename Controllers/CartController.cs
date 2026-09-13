using System.Linq;
using AutoMapper;
using Bookstore.Data;
using Bookstore.DTOs.Cart;
using Bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private int GetUserIdOrThrow()
        {
            var claim = User.FindFirst("uid");
            if (claim == null || !int.TryParse(claim.Value, out var userId) || userId <= 0)
                throw new InvalidOperationException("Brak identyfikatora użytkownika w tokenie.");
            return userId;
        }

        // GET: api/cart
        [HttpGet]
        public ActionResult<CartReadDto> GetCart()
        {
            var userId = GetUserIdOrThrow();
            var cart = _context.Carts
                .Include(c => c.CartItems).ThenInclude(i => i.Book)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null) return NotFound(new { message = "Nie znaleziono koszyka użytkownika." });

            var dto = _mapper.Map<CartReadDto>(cart);
            return Ok(dto);
        }

        // POST: /api/cart/items (zwiększenie liczby egzemplarzy w koszyku)
        [HttpPost("items")]
        public ActionResult<object> AddItemToCart([FromBody] CartItemCreateDto body)
        {
            if (body == null || body.Quantity <= 0)
                return BadRequest(new { message = "Liczba egzemplarzy musi być większa od zera." });

            var userId = GetUserIdOrThrow();
            var cart = _context.Carts
                .Include(c => c.CartItems).ThenInclude(i => i.Book)
                .FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return NotFound(new { message = "Nie znaleziono koszyka użytkownika." });

            var book = _context.Books.FirstOrDefault(b => b.BookId == body.BookId);
            if (book == null) return NotFound(new { message = "Nie znaleziono książki o podanym identyfikatorze." });

            var item = cart.CartItems.FirstOrDefault(i => i.BookId == body.BookId);
            var previous = item?.Quantity ?? 0;
            var newQty = previous + body.Quantity;

            // kontrola stanów magazynowych
            if (book.Stock < newQty)
                return BadRequest(new
                {
                    message = $"Brak wystarczającej liczby egzemplarzy książki „{book.Title}”. " +
                              $"Dostępny stan magazynowy: {book.Stock}."
                });

            if (item == null)
            {
                item = _mapper.Map<CartItem>(body);
                cart.CartItems.Add(item); // EF sam ustawi CartId
            }
            else
            {
                item.Quantity = newQty;
            }

            _context.SaveChanges();

            return Ok(new
            {
                bookId = item.BookId,
                title = book.Title,
                added = body.Quantity,
                previous,
                quantity = item.Quantity,
                price = book.Price
            });
        }

        // PUT: /api/cart/items/{bookId} (ustawienie liczby egzemplarzy)
        [HttpPut("items/{bookId:int}")]
        public IActionResult SetQuantity(int bookId, [FromBody] CartItemCreateDto body)
        {
            if (body == null) return BadRequest(new { message = "Brak danych żądania." });
            var userId = GetUserIdOrThrow();
            var cart = _context.Carts
                .Include(c => c.CartItems).ThenInclude(i => i.Book)
                .FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return NotFound(new { message = "Nie znaleziono koszyka użytkownika." });

            var item = cart.CartItems.FirstOrDefault(i => i.BookId == bookId);
            if (item == null) return NotFound(new { message = "Wskazanej pozycji nie ma w koszyku." });

            if (body.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
                return NoContent();
            }

            // kontrola stanów magazynowych
            if (item.Book.Stock < body.Quantity)
                return BadRequest(new
                {
                    message = $"Brak wystarczającej liczby egzemplarzy książki „{item.Book.Title}”. " +
                              $"Dostępny stan magazynowy: {item.Book.Stock}."
                });

            item.Quantity = body.Quantity;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: /api/cart/items/{bookId}
        [HttpDelete("items/{bookId:int}")]
        public IActionResult RemoveCartItem(int bookId)
        {
            var userId = GetUserIdOrThrow();
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return NotFound(new { message = "Nie znaleziono koszyka użytkownika." });

            var item = cart.CartItems.FirstOrDefault(i => i.BookId == bookId);
            if (item == null) return NotFound(new { message = "Wskazanej pozycji nie ma w koszyku." });

            _context.CartItems.Remove(item);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: /api/cart  -> wyczyść koszyk
        [HttpDelete]
        public IActionResult ClearCart()
        {
            var userId = GetUserIdOrThrow();
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return NotFound(new { message = "Nie znaleziono koszyka użytkownika." });

            if (cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();
            }
            return NoContent();
        }
    }
}
