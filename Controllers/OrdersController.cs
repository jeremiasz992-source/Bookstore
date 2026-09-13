using Bookstore.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Services;
using Bookstore.Services.Interfaces;
using Bookstore.DTOs.Orders;
using Bookstore.DTOs.Payments;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // wymaga ważnego JWT
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orders;
        private readonly IPaymentService _payments;

        public OrdersController(IOrderService orders, IPaymentService payments)
        {
            _orders = orders;
            _payments = payments;
        }

        private int GetUserIdOrThrow()
        {
            var claim = User.FindFirst("uid");
            if (claim == null || !int.TryParse(claim.Value, out var userId) || userId <= 0)
                throw new InvalidOperationException("Brak identyfikatora użytkownika w tokenie.");
            return userId;
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public ActionResult<OrderReadDto> Checkout()
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var dto = _orders.Checkout(userId);
                return Ok(dto);
            }
            catch (ConcurrencyConflictException ex)
            {
                // 409 Conflict
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders
        [HttpGet]
        public ActionResult<IEnumerable<OrderReadDto>> MyOrders()
        {
            var userId = GetUserIdOrThrow();
            var list = _orders.GetMyOrders(userId);
            return Ok(list);
        }

        // GET: api/orders/all (tylko admin)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<OrderReadDto>> AllOrders()
        {
            var list = _orders.GetAllOrders();
            return Ok(list);
        }

        // PUT: api/orders/{id}/status (tylko admin)
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public ActionResult<OrderReadDto> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto body)
        {
            try
            {
                var result = _orders.UpdateStatus(id, body?.Status ?? string.Empty);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/orders/{id}/pay
        [HttpPost("{id:int}/pay")]
        public ActionResult<PaymentReadDto> Pay(int id, [FromBody] PaymentCreateDto body)
        {
            var userId = GetUserIdOrThrow();

            try
            {
                var _ = _payments.Create(id, body?.Method ?? string.Empty, userId);

                var result = _payments.MarkPaid(id, Guid.NewGuid().ToString("N"), userId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
