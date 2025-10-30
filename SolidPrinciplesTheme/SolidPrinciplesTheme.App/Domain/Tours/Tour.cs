namespace SolidPrinciplesTheme.App.Domain.Tours;

/// <summary>
/// Represents a wildlife tour offered by the park.
/// Demonstrates Liskov Substitution when derived tours respect the same contract.
/// </summary>
public abstract class Tour
{
    protected Tour(string code, string name, int durationHours, decimal basePricePerParticipant)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Tour code must be provided", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tour name must be provided", nameof(name));
        }

        if (durationHours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationHours), "Duration must be positive");
        }

        if (basePricePerParticipant <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(basePricePerParticipant), "Price must be positive");
        }

        Code = code;
        Name = name;
        DurationHours = durationHours;
        BasePricePerParticipant = basePricePerParticipant;
    }

    public string Code { get; }

    public string Name { get; }

    public int DurationHours { get; }

    public decimal BasePricePerParticipant { get; }

    public abstract string Terrain { get; }

    public virtual bool CanAccommodateGroup(int participants) => participants is > 0 and <= 12;

    public TourBooking CreateBooking(int participants, DateTime scheduledFor, decimal price) =>
        new(this, participants, scheduledFor, price);
}
