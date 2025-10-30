namespace SolidPrinciplesTheme.App.SolidDemo.Models;

/// <summary>
/// LISKOV SUBSTITUTION PRINCIPLE (LSP) DEMONSTRATION
/// 
/// Base abstract class for different order types.
/// All derived classes must be substitutable for the base class without breaking functionality.
/// Each derived class maintains the contract defined by the base class.
/// </summary>
public abstract class OrderType
{
    protected OrderType(string name, bool requiresShipping, bool isInstantDelivery)
    {
        Name = name;
        RequiresShipping = requiresShipping;
        IsInstantDelivery = isInstantDelivery;
    }

    public string Name { get; }
    public bool RequiresShipping { get; }
    public bool IsInstantDelivery { get; }

    /// <summary>
    /// All derived classes must implement this method and return a positive processing time.
    /// This contract ensures LSP compliance - any derived class can replace the base class.
    /// </summary>
    public abstract TimeSpan GetProcessingTime();

    /// <summary>
    /// All derived classes must validate their specific requirements.
    /// Base contract: method must not throw exceptions for valid orders.
    /// </summary>
    public abstract bool ValidateOrder(Order order);

    /// <summary>
    /// Template method that defines the order fulfillment process.
    /// This ensures consistent behavior across all order types.
    /// </summary>
    public virtual string GetFulfillmentInstructions()
    {
        return $"Process {Name} order following standard procedures.";
    }
}

/// <summary>
/// Physical product orders - require shipping, longer processing time
/// </summary>
public class PhysicalProductOrder : OrderType
{
    public PhysicalProductOrder() : base("Physical Product", requiresShipping: true, isInstantDelivery: false)
    {
    }

    public override TimeSpan GetProcessingTime()
    {
        return TimeSpan.FromDays(1); // Physical products need 1 day to process
    }

    public override bool ValidateOrder(Order order)
    {
        // Physical products require shipping address
        return !string.IsNullOrWhiteSpace(order.ShippingAddress) &&
               order.Items.Any(item => item.ProductType == ProductType.Physical);
    }

    public override string GetFulfillmentInstructions()
    {
        return "Package physical items, print shipping label, schedule pickup with carrier.";
    }
}

/// <summary>
/// Digital product orders - no shipping required, instant delivery
/// </summary>
public class DigitalProductOrder : OrderType
{
    public DigitalProductOrder() : base("Digital Product", requiresShipping: false, isInstantDelivery: true)
    {
    }

    public override TimeSpan GetProcessingTime()
    {
        return TimeSpan.FromMinutes(5); // Digital products process almost instantly
    }

    public override bool ValidateOrder(Order order)
    {
        // Digital products just need valid email for delivery
        return order.Items.Any(item => item.ProductType == ProductType.Digital);
    }

    public override string GetFulfillmentInstructions()
    {
        return "Generate download links, send access credentials via email.";
    }
}

/// <summary>
/// Subscription orders - recurring billing, immediate access
/// </summary>
public class SubscriptionOrder : OrderType
{
    public SubscriptionOrder() : base("Subscription", requiresShipping: false, isInstantDelivery: true)
    {
    }

    public override TimeSpan GetProcessingTime()
    {
        return TimeSpan.FromMinutes(10); // Subscriptions need account setup
    }

    public override bool ValidateOrder(Order order)
    {
        // Subscriptions require payment method for recurring billing
        return !string.IsNullOrWhiteSpace(order.PaymentMethod) &&
               order.Items.Any(item => item.ProductType == ProductType.Subscription);
    }

    public override string GetFulfillmentInstructions()
    {
        return "Activate subscription, set up recurring billing, send welcome email with access details.";
    }
}