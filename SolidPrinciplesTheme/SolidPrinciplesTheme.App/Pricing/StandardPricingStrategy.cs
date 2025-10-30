using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Pricing;

/// <summary>
/// Base strategy; clients can extend behaviour without modifying it (Open/Closed Principle).
/// </summary>
public class StandardPricingStrategy : ITourPricingStrategy
{
    public virtual decimal CalculatePrice(Tour tour, int participants, DateTime scheduledFor)
    {
        var baseTotal = tour.BasePricePerParticipant * participants;
        return Math.Round(baseTotal, 2, MidpointRounding.AwayFromZero);
    }
}
