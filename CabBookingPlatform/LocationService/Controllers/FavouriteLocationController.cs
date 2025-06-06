using Microsoft.AspNetCore.Mvc;
using LocationService.Data;
using LocationService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LocationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteLocationController : ControllerBase
    {
        private readonly LocationDbContext _context;

        public FavouriteLocationController(LocationDbContext context)
        {
            _context = context;
        }

        // GET api/FavouriteLocation/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<FavouriteLocation>>> GetLocations(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("UserId cannot be empty.");

            var locations = await _context.FavouriteLocations
                .Where(l => l.UserId == userId)
                .ToListAsync();

            return Ok(locations ?? new List<FavouriteLocation>());
        }

        // POST api/FavouriteLocation
        [HttpPost]
        public async Task<ActionResult<FavouriteLocation>> AddLocation([FromBody] FavouriteLocation location)
        {
            if (location == null)
                return BadRequest("Location data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsValidCoordinates(location.Latitude, location.Longitude))
                return BadRequest("Invalid latitude or longitude values.");

            try
            {
                // Defensive: explicitly reset ID to avoid conflicts if sent by client
                location.Id = 0;

                _context.FavouriteLocations.Add(location);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLocations), new { userId = location.UserId }, location);
            }
            catch (DbUpdateException dbEx)
            {
                // Log error here (omitted for brevity)
                return StatusCode(500, $"Database error occurred: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Log error here (omitted for brevity)
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        // PUT api/FavouriteLocation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] FavouriteLocation updated)
        {
            if (updated == null)
                return BadRequest("Updated location data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updated.Id)
                return BadRequest("ID mismatch between URL and body.");

            if (!IsValidCoordinates(updated.Latitude, updated.Longitude))
                return BadRequest("Invalid latitude or longitude values.");

            var loc = await _context.FavouriteLocations.FindAsync(id);
            if (loc == null)
                return NotFound($"Location with ID {id} not found.");

            try
            {
                loc.Name = updated.Name;
                loc.Address = updated.Address;
                loc.Latitude = updated.Latitude;
                loc.Longitude = updated.Longitude;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database error occurred: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        // GET api/FavouriteLocation/weather/{locationId}
        [HttpGet("weather/{locationId}")]
        public async Task<ActionResult<JsonDocument>> GetWeather(int locationId, [FromServices] WeatherService weatherService)
        {
            var loc = await _context.FavouriteLocations.FindAsync(locationId);
            if (loc == null)
                return NotFound("Location not found.");

            try
            {
                var weatherJson = await weatherService.GetWeather(loc.Latitude, loc.Longitude);
                return Ok(JsonDocument.Parse(weatherJson));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve weather data: {ex.Message}");
            }
        }

        // DELETE api/FavouriteLocation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var loc = await _context.FavouriteLocations.FindAsync(id);
            if (loc == null)
                return NotFound();

            try
            {
                _context.FavouriteLocations.Remove(loc);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete location: {ex.Message}");
            }
        }

        // Utility method: validates latitude and longitude ranges
        private bool IsValidCoordinates(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }
    }
}
