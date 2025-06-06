using System.Text.Json;
using System.Text;

namespace BookingService.Services
{
    public class EventPublisher : IEventPublisher

    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public EventPublisher(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task PublishBookingCompleted(string userId)
        {
            var endpoint = _config["CustomerService:DiscountNotificationUrl"]; // e.g. http://customerservice/api/notification/trigger-discount
            var content = new StringContent(JsonSerializer.Serialize(new { UserId = userId }), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(endpoint, content);
        }
    }

}
