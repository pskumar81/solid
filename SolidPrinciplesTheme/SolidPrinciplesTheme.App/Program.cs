using SolidPrinciplesTheme.App.Domain.Tours;
using SolidPrinciplesTheme.App.Notifications;
using SolidPrinciplesTheme.App.Policies;
using SolidPrinciplesTheme.App.Pricing;
using SolidPrinciplesTheme.App.Repositories;
using SolidPrinciplesTheme.App.Services;

var repository = new InMemoryTourRepository();

var canopyWalk = new WalkingTour("TRAIL-EXPLORE", "Canopy Walk Expedition", 2, 45m);
var nightLagoon = new BoatTour("RIVER-NIGHT", "Night Lagoon Safari", 3, 65m);

repository.AddTour(canopyWalk);
repository.AddTour(nightLagoon);

var pricingStrategy = new PeakSeasonPricingStrategy(new StandardPricingStrategy());
var policy = new FamilyFriendlyPolicy();
var notifications = new INotificationChannel[]
{
	new EmailNotificationChannel(),
	new SmsNotificationChannel()
};

var bookingService = new TourBookingService(
	repository,
	repository,
	repository,
	pricingStrategy,
	policy,
	notifications);

var availabilityService = new TourAvailabilityService(repository, repository);

var schedule = new DateTime(2025, 10, 30, 9, 0, 0);

if (availabilityService.IsSlotAvailable(canopyWalk.Code, schedule))
{
	var booking = bookingService.Book(canopyWalk.Code, 4, schedule);
	Console.WriteLine("Booking successfully created:");
	Console.WriteLine(booking);
}

Console.WriteLine();
Console.WriteLine("Tours currently offered:");
foreach (var tour in repository.GetAvailableTours())
{
	Console.WriteLine($"- {tour.Name} ({tour.Terrain}) | Duration: {tour.DurationHours}h | Base fee: {tour.BasePricePerParticipant:C}");
}
