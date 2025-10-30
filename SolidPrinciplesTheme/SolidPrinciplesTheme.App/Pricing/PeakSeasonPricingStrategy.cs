using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Pricing;

public sealed class PeakSeasonPricingStrategy : StandardPricingStrategy
{
    private readonly StandardPricingStrategy _fallback;

    public PeakSeasonPricingStrategy(StandardPricingStrategy fallback)
    {
        _fallback = fallback;
    }

    public override decimal CalculatePrice(Tour tour, int participants, DateTime scheduledFor)
    {
        var basePrice = _fallback.CalculatePrice(tour, participants, scheduledFor);

        var isWeekend = scheduledFor.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var isMigrationSeason = tour.Terrain == "River" && scheduledFor.Month is 9 or 10;

        if (isWeekend)
        {
            basePrice *= 1.15m;
        }

        if (isMigrationSeason)
        {
            basePrice *= 1.10m;
        }

        return Math.Round(basePrice, 2, MidpointRounding.AwayFromZero);
    }
}
