namespace BookingService.Events
{
    public class CabReadyNotificationEvent
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime TripDateTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
