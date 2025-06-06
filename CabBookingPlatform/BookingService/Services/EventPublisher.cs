using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

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

        public async Task PublishBookingCompleted(string userId)
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
    }
}
