// Controllers/FavouriteLocationController.cs
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
            var locations = await _context.FavouriteLocations
                .Where(l => l.UserId == userId)
                .ToListAsync();

            // Optionally return 404 if none found, or just return empty list
            if (locations == null || !locations.Any())
            {
                return Ok(new List<FavouriteLocation>()); // or return NotFound();
            }

            return Ok(locations);
        }

        // POST api/FavouriteLocation
        [HttpPost]
        public async Task<ActionResult<FavouriteLocation>> AddLocation([FromBody] FavouriteLocation location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.FavouriteLocations.Add(location);
            await _context.SaveChangesAsync();

            // Return 201 with location of the resource
            return CreatedAtAction(nameof(GetLocations), new { userId = location.UserId }, location);
        }

        // PUT api/FavouriteLocation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] FavouriteLocation updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updated.Id)
                return BadRequest("ID mismatch between URL and body");

            var loc = await _context.FavouriteLocations.FindAsync(id);
            if (loc == null)
                return NotFound();

            loc.Name = updated.Name;
            loc.Address = updated.Address;
            loc.Latitude = updated.Latitude;
            loc.Longitude = updated.Longitude;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET api/FavouriteLocation/weather/{locationId}
        [HttpGet("weather/{locationId}")]
        public async Task<ActionResult<JsonDocument>> GetWeather(int locationId, [FromServices] WeatherService weatherService)
        {
            var loc = await _context.FavouriteLocations.FindAsync(locationId);
            if (loc == null)
                return NotFound("Location not found");

            var weatherJson = await weatherService.GetWeather(loc.Latitude, loc.Longitude);

            // Parse and return as JSON document
            return Ok(JsonDocument.Parse(weatherJson));
        }

        // DELETE api/FavouriteLocation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var loc = await _context.FavouriteLocations.FindAsync(id);
            if (loc == null)
                return NotFound();

            _context.FavouriteLocations.Remove(loc);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
