using Bookstore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bookstore.Data;
using Bookstore.DTOs.Orders;
using Bookstore.Models;
using Bookstore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bookstore.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            AppDbContext ctx,
            IMapper mapper,
            IEmailService emailService,
            ILogger<OrderService> logger)
        {
            _ctx = ctx;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public OrderReadDto Checkout(int userId)
        {
            using var transaction = _ctx.Database.BeginTransaction();

            var cart = _ctx.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems).ThenInclude(ci => ci.Book)
                .SingleOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Koszyk jest pusty.");

            var cartItems = cart.CartItems.ToList();

            foreach (var item in cartItems)
            {
                if (item.Book == null)
                    throw new InvalidOperationException($"Książka o ID {item.BookId} nie istnieje.");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Koszyk zawiera nieprawidłową ilość książek.");

                if (item.Book.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Brak wystarczającej liczby egzemplarzy książki „{item.Book.Title}”. " +
                        $"Dostępny stan magazynowy: {item.Book.Stock}.");
            }

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    Price = ci.Book.Price
                }).ToList()
            };

            foreach (var item in cartItems)
            {
                item.Book.Stock -= item.Quantity;
            }

            var recipientEmail = cart.User.Email;

            _ctx.Orders.Add(order);
            _ctx.CartItems.RemoveRange(cartItems);

            try
            {
                _ctx.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Inny klient zmienił w międzyczasie stan magazynowy jednej z książek.
                // Transakcja zostaje wycofana, więc zamówienie nie powstaje.
                transaction.Rollback();
                throw new ConcurrencyConflictException(
                    "Stan magazynowy jednej z książek zmienił się podczas składania zamówienia. " +
                    "Prosimy sprawdzić zawartość koszyka i spróbować ponownie.");
            }

            // Zatwierdzenie zmian w bazie przed wysłaniem wiadomości e-mail.
            transaction.Commit();

            // Pobranie utworzonego zamówienia wraz z powiązanymi danymi
            var created = _ctx.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Payment)
                .First(o => o.OrderId == order.OrderId);

            var dto = _mapper.Map<OrderReadDto>(created);

            // Błąd wysyłki e-mail nie anuluje złożonego zamówienia.
            try
            {
                _emailService.SendOrderConfirmation(created, recipientEmail);
                dto.ConfirmationEmailSent = true;
                dto.ConfirmationMessage = "Potwierdzenie zamówienia zostało wysłane na adres e-mail klienta.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nie udało się wysłać potwierdzenia zamówienia #{OrderId} na adres {Email}.",
                    created.OrderId, recipientEmail);

                dto.ConfirmationEmailSent = false;
                dto.ConfirmationMessage = "Zamówienie zostało złożone, ale nie udało się wysłać potwierdzenia e-mail. " +
                                          "Szczegóły zamówienia są dostępne w zakładce Zamówienia.";
            }

            return dto;
        }

        public IEnumerable<OrderReadDto> GetMyOrders(int userId)
        {
            var orders = _ctx.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        // Zmiana stanu zamówienia przez administratora.
        public OrderReadDto UpdateStatus(int orderId, string status)
        {
            if (status != "Completed")
                throw new InvalidOperationException("Nieobsługiwany stan zamówienia.");

            var order = _ctx.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Payment)
                .FirstOrDefault(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException("Zamówienie nie istnieje.");

            if (order.Status != "Paid")
                throw new InvalidOperationException("Jako zrealizowane można oznaczyć wyłącznie zamówienie opłacone.");

            order.Status = status;
            _ctx.SaveChanges();

            return _mapper.Map<OrderReadDto>(order);
        }

        public IEnumerable<OrderReadDto> GetAllOrders()
        {
            var orders = _ctx.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }
    }
}

