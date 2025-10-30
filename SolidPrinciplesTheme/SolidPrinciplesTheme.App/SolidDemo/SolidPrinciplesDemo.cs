using SolidPrinciplesTheme.App.SolidDemo.Models;
using SolidPrinciplesTheme.App.SolidDemo.Services;
using SolidPrinciplesTheme.App.SolidDemo.Strategies;
using SolidPrinciplesTheme.App.SolidDemo.Repositories;

namespace SolidPrinciplesTheme.App.SolidDemo;

/// <summary>
/// SOLID PRINCIPLES COMPREHENSIVE DEMONSTRATION
/// 
/// This class demonstrates all five SOLID principles working together in a real-world scenario:
/// An e-commerce order processing system.
/// 
/// The demonstration shows how SOLID principles create:
/// 1. Maintainable code (easy to modify)
/// 2. Extensible code (easy to add new features)
/// 3. Testable code (dependencies can be mocked)
/// 4. Loosely coupled code (changes in one area don't affect others)
/// </summary>
public static class SolidPrinciplesDemo
{
    public static async Task RunDemonstrationAsync()
    {
        Console.WriteLine(ProblemStatement.Description);
        Console.WriteLine();
        
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("SOLID PRINCIPLES DEMONSTRATION: E-COMMERCE ORDER PROCESSING");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        // Set up the dependency injection container (demonstrating DIP)
        var container = SetupDependencies();
        
        // Create sample orders
        var orders = CreateSampleOrders();

        foreach (var (order, orderType, description) in orders)
        {
            Console.WriteLine($"Processing {description}...");
            Console.WriteLine("-".PadRight(60, '-'));
            
            await ProcessOrderWithDifferentStrategies(container, order, orderType);
            Console.WriteLine();
        }

        // Demonstrate extensibility by adding new pricing strategy without modifying existing code
        await DemonstrateExtensibility(container);

        Console.WriteLine();
        Console.WriteLine("Demo completed. All SOLID principles have been demonstrated!");
    }

    /// <summary>
    /// Sets up dependency injection container demonstrating Dependency Inversion Principle
    /// </summary>
    private static DependencyContainer SetupDependencies()
    {
        Console.WriteLine("🔧 Setting up dependencies (Dependency Inversion Principle)...");
        
        // Create concrete implementations
        var repository = new InMemoryOrderRepository();
        var inventoryService = new InventoryService();
        var orderValidator = new OrderValidationService(inventoryService);
        var paymentService = new PaymentService();
        
        // Create notification channels (demonstrating Strategy pattern)
        var notificationChannels = new List<INotificationChannel>
        {
            new EmailNotificationChannel(),
            new SmsNotificationChannel(),
            new PushNotificationChannel()
        };
        var notificationService = new NotificationService(notificationChannels);

        return new DependencyContainer
        {
            Repository = repository,
            InventoryService = inventoryService,
            OrderValidator = orderValidator,
            PaymentService = paymentService,
            NotificationService = notificationService
        };
    }

    /// <summary>
    /// Creates sample orders demonstrating Liskov Substitution Principle
    /// </summary>
    private static List<(Order order, OrderType orderType, string description)> CreateSampleOrders()
    {
        Console.WriteLine("📦 Creating sample orders (Liskov Substitution Principle)...");
        
        var orders = new List<(Order, OrderType, string)>();

        // Physical product order
        var physicalOrder = new Order("ORD-001", "CUST-123", 0m, DateTime.Now);
        physicalOrder.AddItem(new OrderItem("LAPTOP-001", "Gaming Laptop", 1200m, 1, ProductType.Physical));
        physicalOrder.AddItem(new OrderItem("PHONE-002", "Smartphone", 800m, 1, ProductType.Physical));
        physicalOrder.SetShippingAddress("123 Main St, City, State 12345");
        orders.Add((physicalOrder, new PhysicalProductOrder(), "Physical Product Order"));

        // Digital product order
        var digitalOrder = new Order("ORD-002", "CUST-456", 0m, DateTime.Now);
        digitalOrder.AddItem(new OrderItem("SOFTWARE-003", "Photo Editor Pro", 99.99m, 1, ProductType.Digital));
        orders.Add((digitalOrder, new DigitalProductOrder(), "Digital Product Order"));

        // Subscription order
        var subscriptionOrder = new Order("ORD-003", "CUST-789", 0m, DateTime.Now);
        subscriptionOrder.AddItem(new OrderItem("SUBSCRIPTION-004", "Premium Cloud Storage", 9.99m, 12, ProductType.Subscription));
        subscriptionOrder.SetPaymentMethod("creditcard");
        orders.Add((subscriptionOrder, new SubscriptionOrder(), "Subscription Order"));

        return orders;
    }

    /// <summary>
    /// Processes orders with different pricing strategies demonstrating Open/Closed Principle
    /// </summary>
    private static async Task ProcessOrderWithDifferentStrategies(DependencyContainer container, Order order, OrderType orderType)
    {
        // Demonstrate different pricing strategies (Open/Closed Principle)
        var pricingStrategies = new List<IPricingStrategy>
        {
            new StandardPricingStrategy(),
            new BulkDiscountPricingStrategy(1500m, 0.10m),
            new MembershipPricingStrategy(new StandardPricingStrategy(), 0.15m),
            new SeasonalPricingStrategy(new StandardPricingStrategy(), 1.20m, "Holiday"),
            new ProductTypePricingStrategy(new BulkDiscountPricingStrategy())
        };

        Console.WriteLine($"   Order ID: {order.OrderId}");
        Console.WriteLine($"   Customer: {order.CustomerId}");
        Console.WriteLine($"   Items: {order.Items.Count}");
        Console.WriteLine($"   Subtotal: {order.Subtotal:C}");
        Console.WriteLine();

        // Show order type specific information (Liskov Substitution Principle)
        Console.WriteLine("📋 Order Type Analysis:");
        var orderTypeInfo = container.GetOrderProcessingService(pricingStrategies[0])
            .GetOrderTypeSpecificInstructions(orderType, order);
        Console.WriteLine(orderTypeInfo);
        Console.WriteLine();

        // Process with different pricing strategies
        Console.WriteLine("💰 Testing different pricing strategies (Open/Closed Principle):");
        
        foreach (var strategy in pricingStrategies)
        {
            var orderProcessor = container.GetOrderProcessingService(strategy);
            
            var testOrder = CloneOrder(order); // Create a copy for testing
            var result = await orderProcessor.ProcessOrderAsync(testOrder, "creditcard");
            
            Console.WriteLine($"   ✓ {strategy.GetStrategyName()}: {result.TotalAmount:C} " +
                            $"({(result.IsSuccess ? "SUCCESS" : "FAILED")})");
            
            if (!result.IsSuccess)
            {
                Console.WriteLine($"     Error: {result.ErrorMessage}");
            }
        }
    }

    /// <summary>
    /// Demonstrates how easy it is to extend the system with new functionality
    /// without modifying existing code (Open/Closed Principle)
    /// </summary>
    private static async Task DemonstrateExtensibility(DependencyContainer container)
    {
        Console.WriteLine("🔧 EXTENSIBILITY DEMONSTRATION");
        Console.WriteLine("Adding new pricing strategy without modifying existing code...");
        Console.WriteLine();

        // Create a new custom pricing strategy without modifying any existing code
        var customStrategy = new CustomVIPPricingStrategy();
        
        var vipOrder = new Order("ORD-VIP", "VIP-CUSTOMER", 0m, DateTime.Now);
        vipOrder.AddItem(new OrderItem("LAPTOP-001", "Gaming Laptop", 1200m, 2, ProductType.Physical));
        vipOrder.SetShippingAddress("VIP Address");

        var orderProcessor = container.GetOrderProcessingService(customStrategy);
        var result = await orderProcessor.ProcessOrderAsync(vipOrder, "creditcard");

        Console.WriteLine($"🌟 VIP Order Processing:");
        Console.WriteLine($"   Strategy: {customStrategy.GetStrategyName()}");
        Console.WriteLine($"   Total: {result.TotalAmount:C}");
        Console.WriteLine($"   Status: {(result.IsSuccess ? "SUCCESS" : "FAILED")}");
        Console.WriteLine($"   Message: {result.Message}");
    }

    /// <summary>
    /// Helper method to clone an order for testing different strategies
    /// </summary>
    private static Order CloneOrder(Order original)
    {
        var clone = new Order(original.OrderId, original.CustomerId, 0m, original.OrderDate);
        
        foreach (var item in original.Items)
        {
            clone.AddItem(new OrderItem(item.ProductId, item.ProductName, item.Price, item.Quantity, item.ProductType));
        }
        
        if (!string.IsNullOrEmpty(original.ShippingAddress))
            clone.SetShippingAddress(original.ShippingAddress);
        
        if (!string.IsNullOrEmpty(original.PaymentMethod))
            clone.SetPaymentMethod(original.PaymentMethod);

        return clone;
    }
}

/// <summary>
/// Custom VIP pricing strategy - demonstrates extensibility without modification
/// </summary>
public class CustomVIPPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(Order order)
    {
        var basePrice = order.Subtotal;
        
        // VIP customers get free shipping (save $25) and 20% discount
        var vipDiscount = basePrice * 0.20m;
        var freeShipping = 25m;
        
        return Math.Max(0, basePrice - vipDiscount - freeShipping);
    }

    public string GetStrategyName() => "VIP Customer Pricing (20% off + Free Shipping)";
}

/// <summary>
/// Dependency container for demonstrating Dependency Inversion Principle
/// </summary>
public class DependencyContainer
{
    public InMemoryOrderRepository Repository { get; set; } = null!;
    public InventoryService InventoryService { get; set; } = null!;
    public OrderValidationService OrderValidator { get; set; } = null!;
    public PaymentService PaymentService { get; set; } = null!;
    public NotificationService NotificationService { get; set; } = null!;

    public OrderProcessingService GetOrderProcessingService(IPricingStrategy pricingStrategy)
    {
        return new OrderProcessingService(
            OrderValidator,
            Repository,
            Repository,
            PaymentService,
            NotificationService,
            InventoryService,
            pricingStrategy
        );
    }
}