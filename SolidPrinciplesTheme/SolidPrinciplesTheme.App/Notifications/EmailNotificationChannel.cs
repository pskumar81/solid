using SolidPrinciplesTheme.App.Domain;

namespace SolidPrinciplesTheme.App.Notifications;

public sealed class EmailNotificationChannel : INotificationChannel
{
    public void NotifyBookingCreated(TourBooking booking)
    {
        Console.WriteLine($"[Email] Confirmation {booking.ConfirmationCode} sent to guests of {booking.Tour.Name}.");
    }
}
