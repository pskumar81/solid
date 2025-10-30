using SolidPrinciplesTheme.App.Domain;
using SolidPrinciplesTheme.App.Domain.Tours;
using SolidPrinciplesTheme.App.Notifications;
using SolidPrinciplesTheme.App.Policies;
using SolidPrinciplesTheme.App.Pricing;
using SolidPrinciplesTheme.App.Repositories;

namespace SolidPrinciplesTheme.App.Services;

/// <summary>
/// Coordinates booking steps while depending on abstractions (Dependency Inversion Principle).
/// </summary>
public sealed class TourBookingService
{
    private readonly ITourLookup _tourLookup;
    private readonly ITourBookingWriter _bookingWriter;
    private readonly ITourScheduleReader _scheduleReader;
    private readonly ITourPricingStrategy _pricingStrategy;
    private readonly IBookingPolicy _bookingPolicy;
    private readonly IEnumerable<INotificationChannel> _notificationChannels;

    public TourBookingService(
        ITourLookup tourLookup,
        ITourBookingWriter bookingWriter,
        ITourScheduleReader scheduleReader,
        ITourPricingStrategy pricingStrategy,
        IBookingPolicy bookingPolicy,
        IEnumerable<INotificationChannel> notificationChannels)
    {
        _tourLookup = tourLookup;
        _bookingWriter = bookingWriter;
        _scheduleReader = scheduleReader;
        _pricingStrategy = pricingStrategy;
        _bookingPolicy = bookingPolicy;
        _notificationChannels = notificationChannels;
    }

    public TourBooking Book(string tourCode, int participants, DateTime scheduledFor)
    {
        var tour = _tourLookup.FindByCode(tourCode) ?? throw new InvalidOperationException($"Tour {tourCode} not found.");

        EnsureSlotAvailable(tour, scheduledFor);
        _bookingPolicy.EnsureCanBook(tour, participants, scheduledFor);

        var price = _pricingStrategy.CalculatePrice(tour, participants, scheduledFor);
        var booking = tour.CreateBooking(participants, scheduledFor, price);

        _bookingWriter.Save(booking);

        foreach (var channel in _notificationChannels)
        {
            channel.NotifyBookingCreated(booking);
        }

        return booking;
    }

    private void EnsureSlotAvailable(Tour tour, DateTime scheduledFor)
    {
        var existing = _scheduleReader.GetBookings(tour.Code);
        if (existing.Any(booking => booking.ScheduledFor == scheduledFor))
        {
            throw new InvalidOperationException($"{tour.Name} is already booked for {scheduledFor:MMM dd yyyy HH:mm}.");
        }
    }
}
