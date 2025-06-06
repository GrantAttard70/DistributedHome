using Microsoft.AspNetCore.Mvc;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.DTOs;
using CustomerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        private readonly AuthService _authService;

        public CustomerController(CustomerDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == req.Email))
                return BadRequest("Email already in use.");

            var customer = new Customer
            {
                FirstName = req.FirstName,
                Surname = req.Surname,
                Email = req.Email,
            };

            customer.PasswordHash = _authService.HashPassword(customer, req.Password);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                Surname = customer.Surname,
                Email = customer.Email
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Email == req.Email);
            if (customer == null || !_authService.VerifyPassword(customer, req.Password))
                return Unauthorized("Invalid email or password.");

            var token = _authService.GenerateJwtToken(customer);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("nameid claim missing");
                return Unauthorized();
            }

            if (!int.TryParse(userId, out var id))
            {
                Console.WriteLine($"Invalid user ID format: {userId}");
                return Unauthorized();
            }

            var customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : Ok(new CustomerResponse(customer));
        }


        [Authorize]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var notifications = await _context.Notifications
                .Where(n => n.CustomerId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

    }
}
