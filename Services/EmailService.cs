using System.Net;
using System.Net.Mail;
using System.Text;
using Bookstore.Helpers;
using Bookstore.Models;
using Bookstore.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Bookstore.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public void SendOrderConfirmation(Order order, string recipientEmail)
        {
            Send(recipientEmail, $"Potwierdzenie zamówienia #{order.OrderId}", BuildBody(order));
        }

        public void SendPaymentConfirmation(Order order, string recipientEmail)
        {
            Send(recipientEmail, $"Potwierdzenie płatności - zamówienie #{order.OrderId}", BuildPaymentBody(order));
        }

        private void Send(string recipientEmail, string subject, string body)
        {
            ValidateSettings();

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            message.To.Add(recipientEmail);

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            smtp.Send(message);
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_settings.Host) ||
                string.IsNullOrWhiteSpace(_settings.Username) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                throw new InvalidOperationException("Brak konfiguracji SMTP. Uzupełnij sekcję Smtp w pliku konfiguracyjnym albo w zmiennych środowiskowych.");
            }
        }

        private static string BuildBody(Order order)
        {
            var lines = new List<string>
            {
                $"Dziękujemy za złożenie zamówienia #{order.OrderId}.",
                $"Data zamówienia: {FormatPolishTime(order.CreatedAt)}",
                $"Status: {DescribeOrderStatus(order.Status)}",
                "",
                "Pozycje:"
            };

            AppendItems(lines, order);

            lines.Add("");
            lines.Add("Bookstore");

            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildPaymentBody(Order order)
        {
            var method = order.Payment?.Method ?? PaymentMethod.Card;

            var lines = new List<string>
            {
                $"Potwierdzamy otrzymanie płatności za zamówienie #{order.OrderId}.",
                $"Forma płatności: {DescribeMethod(method)}.",
                "Status: opłacone",
                $"Data potwierdzenia: {FormatPolishTime(DateTime.UtcNow)}",
                "",
                "Pozycje:"
            };

            AppendItems(lines, order);

            lines.Add("");
            lines.Add("Zamówienie zostanie wysłane w ciągu tygodnia.");
            lines.Add("");
            lines.Add("Bookstore");

            return string.Join(Environment.NewLine, lines);
        }

        // Nazwy metod płatności prezentowane użytkownikowi.
        private static string DescribeMethod(PaymentMethod method) => method switch
        {
            PaymentMethod.Card => "karta płatnicza",
            PaymentMethod.Blik => "BLIK",
            PaymentMethod.Transfer => "przelew bankowy",
            _ => method.ToString()
        };

        // Statusy zamówienia prezentowane użytkownikowi.
        private static string DescribeOrderStatus(string status) => status switch
        {
            "Pending" => "nieopłacone",
            "Paid" => "opłacone",
            _ => status
        };

        private static void AppendItems(List<string> lines, Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var title = item.Book?.Title ?? $"Książka #{item.BookId}";
                var total = item.Price * item.Quantity;
                lines.Add($"- {title} x {item.Quantity} - {total:0.00} PLN");
            }

            lines.Add("");
            lines.Add($"Razem: {order.OrderItems.Sum(i => i.Price * i.Quantity):0.00} PLN");
        }

        // Konwersja czasu UTC na czas CET/CEST.
        private static string FormatPolishTime(DateTime utcTime)
        {
            var utc = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            }

            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            var label = tz.IsDaylightSavingTime(local) ? "CEST" : "CET";
            return $"{local:yyyy-MM-dd HH:mm} ({label})";
        }
    }
}
