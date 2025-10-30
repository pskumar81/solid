using SolidPrinciplesTheme.App.Domain;
using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Repositories;

public sealed class InMemoryTourRepository : ITourLookup, ITourBookingWriter, ITourScheduleReader
{
    private readonly Dictionary<string, Tour> _tours = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<TourBooking> _bookings = new();

    public void AddTour(Tour tour)
    {
        if (!_tours.TryAdd(tour.Code, tour))
        {
            throw new InvalidOperationException($"Tour with code {tour.Code} already exists.");
        }
    }

    public Tour? FindByCode(string code) => _tours.TryGetValue(code, out var tour) ? tour : null;

    public IReadOnlyCollection<Tour> GetAvailableTours() => _tours.Values.ToList();

    public void Save(TourBooking booking)
    {
        _bookings.Add(booking);
    }

    public IReadOnlyCollection<TourBooking> GetBookings(string tourCode) =>
        _bookings.Where(b => string.Equals(b.Tour.Code, tourCode, StringComparison.OrdinalIgnoreCase)).ToList();
}
