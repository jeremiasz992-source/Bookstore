using System.Linq;
using Bookstore.Data;
using Bookstore.DTOs.Auth;
using Bookstore.Models;
using Bookstore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly ITokenService _tokens;

        // Haszowanie haseł
        private static readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(AppDbContext ctx, ITokenService tokens)
        {
            _ctx = ctx;
            _tokens = tokens;
        }

        // Weryfikacja hasła względem skrótu zapisanego w bazie.
        private static bool VerifyPassword(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result != PasswordVerificationResult.Failed;
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest req)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Email == req.Email);
            // Ten sam komunikat dla błędnego adresu e-mail i hasła, aby nie ujawniać, które z nich jest niepoprawne.
            if (user == null || !VerifyPassword(user, req.Password))
                return Unauthorized(new { message = "Nieprawidłowy adres e-mail lub hasło." });

            var jwt = _tokens.CreateToken(user.UserId, user.Email, user.Name, user.Role);

            return Ok(new AuthResponse
            {
                Token = jwt,
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            });
        }
        [HttpPost("register")]
        public ActionResult<AuthResponse> Register([FromBody] RegisterRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Podaj adres e-mail oraz hasło." });

            var email = req.Email.Trim();
            var exists = _ctx.Users.Any(u => u.Email == email);
            if (exists)
                return BadRequest(new { message = "Konto z tym adresem e-mail już istnieje." });

            var user = new User
            {
                Email = email,
                Name = email,
                Role = "Client",
                Cart = new Cart()
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);

            _ctx.Users.Add(user);
            _ctx.SaveChanges();

            var jwt = _tokens.CreateToken(user.UserId, user.Email, user.Name, user.Role);

            return CreatedAtAction(nameof(Register), new AuthResponse
            {
                Token = jwt,
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            });
        }
    }
}
