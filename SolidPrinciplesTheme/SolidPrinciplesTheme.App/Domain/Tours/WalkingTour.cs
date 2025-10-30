namespace SolidPrinciplesTheme.App.Domain.Tours;

/// <summary>
/// Walking tour keeps the same guarantees as the base Tour to satisfy Liskov Substitution.
/// </summary>
public sealed class WalkingTour : Tour
{
    public WalkingTour(string code, string name, int durationHours, decimal basePricePerParticipant)
        : base(code, name, durationHours, basePricePerParticipant)
    {
    }

    public override string Terrain => "Trail";

    public override bool CanAccommodateGroup(int participants) => participants is > 0 and <= 8;
}
