using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Policies;

/// <summary>
/// Validates bookings without exposing persistence or pricing concerns (Single Responsibility).
/// </summary>
public sealed class FamilyFriendlyPolicy : IBookingPolicy
{
    private static readonly TimeOnly OpeningHour = new(8, 0);
    private static readonly TimeOnly LastDeparture = new(17, 0);

    public void EnsureCanBook(Tour tour, int participants, DateTime scheduledFor)
    {
        if (!tour.CanAccommodateGroup(participants))
        {
            throw new InvalidOperationException($"{tour.Name} cannot host {participants} explorer(s).");
        }

        var departure = TimeOnly.FromDateTime(scheduledFor);
        if (departure < OpeningHour || departure > LastDeparture)
        {
            throw new InvalidOperationException("Tours must depart between 08:00 and 17:00.");
        }

        if (tour.Terrain == "River" && scheduledFor.Month is < 3 or > 11)
        {
            throw new InvalidOperationException("River tours operate from March to November only.");
        }
    }
}
