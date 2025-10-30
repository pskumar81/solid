using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Repositories;

/// <summary>
/// Interface segregation keeps read-only needs separate from write responsibilities.
/// </summary>
public interface ITourLookup
{
    Tour? FindByCode(string code);
    IReadOnlyCollection<Tour> GetAvailableTours();
}
