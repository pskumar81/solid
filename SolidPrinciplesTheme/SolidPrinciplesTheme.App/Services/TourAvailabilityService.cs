using SolidPrinciplesTheme.App.Repositories;

namespace SolidPrinciplesTheme.App.Services;

/// <summary>
/// Dedicated to availability checks only (Single Responsibility Principle).
/// </summary>
public sealed class TourAvailabilityService
{
    private readonly ITourLookup _tourLookup;
    private readonly ITourScheduleReader _scheduleReader;

    public TourAvailabilityService(ITourLookup tourLookup, ITourScheduleReader scheduleReader)
    {
        _tourLookup = tourLookup;
        _scheduleReader = scheduleReader;
    }

    public bool IsSlotAvailable(string tourCode, DateTime scheduledFor)
    {
        var tour = _tourLookup.FindByCode(tourCode) ?? throw new InvalidOperationException($"Tour {tourCode} not found.");
        var existing = _scheduleReader.GetBookings(tour.Code);
        return existing.All(booking => booking.ScheduledFor != scheduledFor);
    }
}
