using SolidPrinciplesTheme.App.Domain.Tours;

namespace SolidPrinciplesTheme.App.Domain;

/// <summary>
/// Represents the outcome of a successful tour booking transaction.
/// </summary>
public sealed class TourBooking
{
    public TourBooking(Tour tour, int participants, DateTime scheduledFor, decimal price)
    {
        Tour = tour ?? throw new ArgumentNullException(nameof(tour));

        if (participants <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(participants), "Participants must be positive");
        }

        Participants = participants;
        ScheduledFor = scheduledFor;
        Price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
        ConfirmationCode = GenerateConfirmationCode(tour, scheduledFor);
    }

    public Tour Tour { get; }

    public int Participants { get; }

    public DateTime ScheduledFor { get; }

    public decimal Price { get; }

    public string ConfirmationCode { get; }

    private static string GenerateConfirmationCode(Tour tour, DateTime scheduledFor)
    {
        var timestamp = scheduledFor.ToString("yyyyMMddHHmm");
        return $"{tour.Code}-{timestamp}";
    }

    public override string ToString() =>
        $"{Tour.Name} on {ScheduledFor:MMM dd yyyy HH:mm} for {Participants} explorer(s) | Total: {Price:C}";
}
