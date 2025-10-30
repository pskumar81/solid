using SolidPrinciplesTheme.App.Domain;

namespace SolidPrinciplesTheme.App.Repositories;

public interface ITourBookingWriter
{
    void Save(TourBooking booking);
}
