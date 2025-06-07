using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BookingService.Events;

namespace BookingService.Services
{
    public class EventPublisher : IEventPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(HttpClient httpClient, IConfiguration config, ILogger<EventPublisher> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task PublishBookingCompleted(int userId)
        {
            var endpoint = _config["CustomerService:DiscountNotificationUrl"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("DiscountNotificationUrl is not configured.");
                return;
            }

            var payload = new { UserId = userId };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Notification failed for user {UserId}. Status code: {StatusCode}", userId, response.StatusCode);
                }
                else
                {
                    _logger.LogInformation("Discount notification sent for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending discount notification for user {UserId}", userId);
            }
        }

        public async Task PublishCabReadyNotification(CabReadyNotificationEvent notification)
        {
            var endpoint = _config["CustomerService:CabReadyNotificationUrl"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("CabReadyNotificationUrl is not configured.");
                return;
            }

            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Cab ready notification failed for booking {BookingId}. Status code: {StatusCode}", notification.BookingId, response.StatusCode);
                }
                else
                {
                    _logger.LogInformation("Cab ready notification sent for booking {BookingId}", notification.BookingId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending cab ready notification for booking {BookingId}", notification.BookingId);
            }
        }
    }
}
