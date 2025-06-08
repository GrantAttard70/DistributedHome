using BookingService.Data;
using BookingService.Models;
using BookingService.Services;
using BookingService.Events;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IEventPublisher _eventPublisher; 

        public BookingController(BookingDbContext context, IEventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher; 
        }


        // POST: api/Booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(180)); //3 minutes
                Console.WriteLine("Notif sent");

                var cabReadyEvent = new CabReadyNotificationEvent
                {
                    BookingId = booking.Id,
                    CustomerId = int.Parse(booking.CustomerId),
                    StartLocation = booking.StartLocation,
                    EndLocation = booking.EndLocation,
                    TripDateTime = booking.TripDateTime,
                    Message = $"Your cab is ready to pick you up at {booking.StartLocation} for your trip to {booking.EndLocation}."
                };


                await _eventPublisher.PublishCabReadyNotification(cabReadyEvent);
            });

            // Check total bookings for user
            var count = await _context.Bookings.CountAsync(b => b.CustomerId == booking.CustomerId);
            if (count == 3)
            {
                await _eventPublisher.PublishBookingCompleted(int.Parse(booking.CustomerId));

            }

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
