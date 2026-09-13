using System;
using System.Collections.Generic;
using System.Linq;
using Bookstore.DTOs.Payments;
using Bookstore.Models;
using Bookstore.Services.Interfaces;
using Bookstore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bookstore.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _ctx;
        private readonly IEmailService _email;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(AppDbContext ctx, IEmailService email, ILogger<PaymentService> logger)
        {
            _ctx = ctx;
            _email = email;
            _logger = logger;
        }

        public PaymentReadDto Create(int orderId, string method, int userId)
        {
            var order = _ctx.Orders
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                throw new KeyNotFoundException("Zamówienie nie istnieje.");

            var methodEnum = method?.Trim().ToLowerInvariant() switch
            {
                "card" => PaymentMethod.Card,
                "blik" => PaymentMethod.Blik,
                "transfer" => PaymentMethod.Transfer,
                _ => throw new InvalidOperationException("Nieobsługiwana metoda płatności.")
            };

            // Obliczenie łącznej wartości pozycji zamówienia.
            var amount = _ctx.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Sum(oi => (decimal?)(oi.Price * (decimal)oi.Quantity)) ?? 0m;

            var payment = _ctx.Payments.FirstOrDefault(p => p.OrderId == orderId);
            if (payment == null)
            {
                payment = new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    Method = methodEnum,
                    Status = PaymentStatus.Pending
                };

                _ctx.Payments.Add(payment);
                _ctx.SaveChanges();
            }

            return new PaymentReadDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Method = payment.Method.ToString(),
                Status = payment.Status.ToString()
            };
        }

        public PaymentReadDto MarkPaid(int orderId, string providerTxnId, int userId)
        {
            var order = _ctx.Orders
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId)
                ?? throw new KeyNotFoundException("Zamówienie nie istnieje.");

            var payment = _ctx.Payments
                .FirstOrDefault(p => p.OrderId == orderId)
                ?? throw new InvalidOperationException("Brak płatności dla tego zamówienia.");

            if (string.IsNullOrWhiteSpace(providerTxnId))
                providerTxnId = Guid.NewGuid().ToString("N");

                // Ponowne wywołanie nie zmienia płatności, jeśli została już zarejestrowana.
            if (string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            {
                payment.ProviderTransactionId = providerTxnId;
                payment.Status = PaymentStatus.Paid;
                order.Status = "Paid";

                // Płatność jest zapisywana przed próbą wysłania potwierdzenia e-mail.
                _ctx.SaveChanges();

                var fullOrder = _ctx.Orders
                    .Include(o => o.User)
                    .Include(o => o.Payment)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                    .First(o => o.OrderId == orderId);

                // Błąd wysyłki e-mail nie wpływa na zapisaną płatność.
                try
                {
                    _email.SendPaymentConfirmation(fullOrder, fullOrder.User.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Nie udało się wysłać potwierdzenia płatności dla zamówienia #{OrderId}.", orderId);
                }
            }

            return new PaymentReadDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Method = payment.Method.ToString(),
                Status = payment.Status.ToString()
            };
        }
    }
}
