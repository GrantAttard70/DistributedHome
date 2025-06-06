using CustomerService.Data;
using CustomerService.DTOs;
using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public NotificationController(CustomerDbContext context)
    {
        _context = context;
    }


    [HttpPost("trigger-discount")]
    public async Task<IActionResult> TriggerDiscount([FromBody] DiscountEventRequest data)
    {
        if (string.IsNullOrWhiteSpace(data.UserId))
            return BadRequest("Missing userId");

        if (!int.TryParse(data.UserId, out int userId))
            return BadRequest("Invalid userId format");

        bool alreadyNotified = await _context.Notifications
            .AnyAsync(n => n.CustomerId == userId && n.Message.Contains("discount"));

        if (alreadyNotified)
            return Ok("Notification already sent");

        _context.Notifications.Add(new Notification
        {
            CustomerId = userId,
            Message = "Congratulations! You’ve unlocked a 10% discount for your next ride."
        });

        await _context.SaveChangesAsync();
        return Ok("Discount notification sent");
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserNotifications(string userId)
    {
        if (!int.TryParse(userId, out int userIdInt))
            return BadRequest("Invalid userId format");

        var notifications = await _context.Notifications
            .Where(n => n.CustomerId == userIdInt)
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

public class DiscountEventRequest
{
    public string UserId { get; set; } = null!;
}

