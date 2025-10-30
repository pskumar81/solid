using SolidPrinciplesTheme.App.SolidDemo.Models;

namespace SolidPrinciplesTheme.App.SolidDemo.Strategies;

/// <summary>
/// OPEN/CLOSED PRINCIPLE (OCP) DEMONSTRATION
/// 
/// Strategy pattern implementation for pricing calculations.
/// The system is OPEN for extension (new pricing strategies) but CLOSED for modification.
/// New pricing strategies can be added without changing existing code.
/// </summary>
public interface IPricingStrategy
{
    decimal CalculatePrice(Order order);
    string GetStrategyName();
}

/// <summary>
/// Standard pricing - base implementation
/// </summary>
public class StandardPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(Order order)
    {
        return order.Subtotal;
    }

    public string GetStrategyName() => "Standard Pricing";
}

/// <summary>
/// Bulk discount pricing - extends functionality without modifying existing code
/// </summary>
public class BulkDiscountPricingStrategy : IPricingStrategy
{
    private readonly decimal _discountThreshold;
    private readonly decimal _discountPercentage;

    public BulkDiscountPricingStrategy(decimal discountThreshold = 100m, decimal discountPercentage = 0.10m)
    {
        _discountThreshold = discountThreshold;
        _discountPercentage = discountPercentage;
    }

    public decimal CalculatePrice(Order order)
    {
        var basePrice = order.Subtotal;
        
        if (basePrice >= _discountThreshold)
        {
            var discount = basePrice * _discountPercentage;
            return basePrice - discount;
        }
        
        return basePrice;
    }

    public string GetStrategyName() => $"Bulk Discount ({_discountPercentage:P} off orders over {_discountThreshold:C})";
}

/// <summary>
/// Membership discount pricing - another extension without modification
/// </summary>
public class MembershipPricingStrategy : IPricingStrategy
{
    private readonly decimal _memberDiscountPercentage;
    private readonly IPricingStrategy _basePricingStrategy;

    public MembershipPricingStrategy(IPricingStrategy basePricingStrategy, decimal memberDiscountPercentage = 0.15m)
    {
        _basePricingStrategy = basePricingStrategy;
        _memberDiscountPercentage = memberDiscountPercentage;
    }

    public decimal CalculatePrice(Order order)
    {
        var basePrice = _basePricingStrategy.CalculatePrice(order);
        var memberDiscount = basePrice * _memberDiscountPercentage;
        return basePrice - memberDiscount;
    }

    public string GetStrategyName() => $"Member Pricing ({_memberDiscountPercentage:P} off) + {_basePricingStrategy.GetStrategyName()}";
}

/// <summary>
/// Seasonal pricing - demonstrates decorator pattern with OCP
/// </summary>
public class SeasonalPricingStrategy : IPricingStrategy
{
    private readonly IPricingStrategy _basePricingStrategy;
    private readonly decimal _seasonalMultiplier;
    private readonly string _seasonName;

    public SeasonalPricingStrategy(IPricingStrategy basePricingStrategy, decimal seasonalMultiplier, string seasonName)
    {
        _basePricingStrategy = basePricingStrategy;
        _seasonalMultiplier = seasonalMultiplier;
        _seasonName = seasonName;
    }

    public decimal CalculatePrice(Order order)
    {
        var basePrice = _basePricingStrategy.CalculatePrice(order);
        return basePrice * _seasonalMultiplier;
    }

    public string GetStrategyName() => $"{_seasonName} Pricing ({_seasonalMultiplier:P} adjustment) + {_basePricingStrategy.GetStrategyName()}";
}

/// <summary>
/// Strategy for different product types - showcases polymorphism with OCP
/// </summary>
public class ProductTypePricingStrategy : IPricingStrategy
{
    private readonly Dictionary<ProductType, decimal> _productTypeMultipliers;
    private readonly IPricingStrategy _basePricingStrategy;

    public ProductTypePricingStrategy(IPricingStrategy basePricingStrategy)
    {
        _basePricingStrategy = basePricingStrategy;
        _productTypeMultipliers = new Dictionary<ProductType, decimal>
        {
            { ProductType.Physical, 1.0m },      // No adjustment for physical products
            { ProductType.Digital, 0.9m },       // 10% discount for digital products
            { ProductType.Subscription, 1.1m }   // 10% premium for subscriptions
        };
    }

    public decimal CalculatePrice(Order order)
    {
        var basePrice = _basePricingStrategy.CalculatePrice(order);
        var totalAdjustment = 0m;

        foreach (var item in order.Items)
        {
            var itemTotal = item.Price * item.Quantity;
            var multiplier = _productTypeMultipliers.GetValueOrDefault(item.ProductType, 1.0m);
            var adjustment = itemTotal * (multiplier - 1.0m);
            totalAdjustment += adjustment;
        }

        return basePrice + totalAdjustment;
    }

    public string GetStrategyName() => $"Product Type Adjusted Pricing + {_basePricingStrategy.GetStrategyName()}";
}