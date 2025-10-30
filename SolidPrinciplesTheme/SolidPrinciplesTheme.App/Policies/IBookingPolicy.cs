using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Policies;

public interface IBookingPolicy
{
    void EnsureCanBook(Tour tour, int participants, DateTime scheduledFor);
}
