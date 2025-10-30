namespace SolidPrinciplesTheme.App.Domain.Tours;

public sealed class BoatTour : Tour
{
    public BoatTour(string code, string name, int durationHours, decimal basePricePerParticipant)
        : base(code, name, durationHours, basePricePerParticipant)
    {
    }

    public override string Terrain => "River";

    public override bool CanAccommodateGroup(int participants) => participants is > 0 and <= 10;
}
