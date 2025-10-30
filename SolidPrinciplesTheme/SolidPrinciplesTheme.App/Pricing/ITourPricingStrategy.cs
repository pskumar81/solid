using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Pricing;

public interface ITourPricingStrategy
{
    decimal CalculatePrice(Tour tour, int participants, DateTime scheduledFor);
}
