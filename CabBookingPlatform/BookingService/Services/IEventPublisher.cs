using BookingService.Events;

public interface IEventPublisher
{
    Task PublishBookingCompleted(int userId);

    Task PublishCabReadyNotification(CabReadyNotificationEvent notification); 
}
