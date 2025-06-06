namespace BookingService.Services
{
    public interface IEventPublisher
    {
        Task PublishBookingCompleted(string userId);
    }
}
