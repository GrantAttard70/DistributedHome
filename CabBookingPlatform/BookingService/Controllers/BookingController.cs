using BookingService.Data;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public BookingController(BookingDbContext context)
        {
            _context = context;
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // GET: api/Booking/current
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentBookings()
        {
            var current = await _context.Bookings
                .Where(b => b.TripDateTime >= DateTime.Now)
                .ToListAsync();
            return Ok(current);
        }

        // GET: api/Booking/past
        [HttpGet("past")]
        public async Task<IActionResult> GetPastBookings()
        {
            var past = await _context.Bookings
                .Where(b => b.TripDateTime < DateTime.Now)
                .ToListAsync();
            return Ok(past);
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }
    }
}
