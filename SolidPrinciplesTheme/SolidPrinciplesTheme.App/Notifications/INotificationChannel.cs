using SolidPrinciplesTheme.App.Domain;

namespace SolidPrinciplesTheme.App.Notifications;

public interface INotificationChannel
{
    void NotifyBookingCreated(TourBooking booking);
}
