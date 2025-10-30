using SolidPrinciplesTheme.App.Domain;

namespace SolidPrinciplesTheme.App.Notifications;

public sealed class SmsNotificationChannel : INotificationChannel
{
    public void NotifyBookingCreated(TourBooking booking)
    {
        Console.WriteLine($"[SMS] {booking.Tour.Name} reserved. Meet at visitor center by {booking.ScheduledFor:HH:mm}.");
    }
}
