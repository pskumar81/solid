using SolidPrinciplesTheme.App.Domain;

namespace SolidPrinciplesTheme.App.Repositories;

public interface ITourScheduleReader
{
    IReadOnlyCollection<TourBooking> GetBookings(string tourCode);
}
