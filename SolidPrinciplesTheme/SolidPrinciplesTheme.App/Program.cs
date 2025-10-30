using SolidPrinciplesTheme.App.Domain.Tours;
using SolidPrinciplesTheme.App.Notifications;
using SolidPrinciplesTheme.App.Policies;
using SolidPrinciplesTheme.App.Pricing;
using SolidPrinciplesTheme.App.Repositories;
using SolidPrinciplesTheme.App.Services;
using SolidPrinciplesTheme.App.SolidDemo;

Console.WriteLine("SOLID PRINCIPLES DEMONSTRATION");
Console.WriteLine("==============================");
Console.WriteLine();
Console.WriteLine("Choose which demonstration to run:");
Console.WriteLine("1. Tour Booking System (Original Demo)");
Console.WriteLine("2. E-Commerce Order Processing (Comprehensive SOLID Demo)");
Console.WriteLine("3. Both Demonstrations");
Console.WriteLine();
Console.Write("Enter your choice (1, 2, or 3): ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await RunTourBookingDemo();
        break;
    case "2":
        await SolidPrinciplesDemo.RunDemonstrationAsync();
        break;
    case "3":
        await RunTourBookingDemo();
        Console.WriteLine("\n" + "=".PadRight(80, '='));
        Console.WriteLine("SWITCHING TO COMPREHENSIVE SOLID DEMO");
        Console.WriteLine("=".PadRight(80, '=') + "\n");
        await SolidPrinciplesDemo.RunDemonstrationAsync();
        break;
    default:
        Console.WriteLine("Invalid choice. Running comprehensive demo...");
        await SolidPrinciplesDemo.RunDemonstrationAsync();
        break;
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

static async Task RunTourBookingDemo()
{
    Console.WriteLine("TOUR BOOKING SYSTEM DEMO");
    Console.WriteLine("========================");
    Console.WriteLine();

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
}
