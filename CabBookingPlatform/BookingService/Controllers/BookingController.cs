using BookingService.Data;
using BookingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingDbContext _context;

    public BookingController(BookingDbContext context)
    {
        _context = context;
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            Console.WriteLine("[BookingController] User ID claim is missing.");
            return false;
        }

        if (!int.TryParse(userIdClaim, out userId))
        {
            Console.WriteLine($"[BookingController] User ID claim is not a valid int: '{userIdClaim}'");
            return false;
        }

        Console.WriteLine($"[BookingController] User ID parsed successfully: {userId}");
        return true;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
    {
        if (!TryGetUserId(out int customerId))
            return Unauthorized("Invalid user ID.");

        booking.CustomerId = customerId;

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(booking);
    }

    [Authorize]
    [HttpGet("current")]
    public IActionResult GetCurrentBookings()
    {
        if (!TryGetUserId(out int customerId))
            return Unauthorized("Invalid user ID.");

        var current = _context.Bookings
            .Where(b => b.CustomerId == customerId && b.BookingTime >= DateTime.UtcNow)
            .OrderBy(b => b.BookingTime)
            .ToList();

        return Ok(current);
    }

    [Authorize]
    [HttpGet("past")]
    public IActionResult GetPastBookings()
    {
        if (!TryGetUserId(out int customerId))
            return Unauthorized("Invalid user ID.");

        var past = _context.Bookings
            .Where(b => b.CustomerId == customerId && b.BookingTime < DateTime.UtcNow)
            .OrderByDescending(b => b.BookingTime)
            .ToList();

        return Ok(past);
    }
}
