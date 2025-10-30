using SolidPrinciplesTheme.App.SolidDemo.Models;
using SolidPrinciplesTheme.App.SolidDemo.Interfaces;

namespace SolidPrinciplesTheme.App.SolidDemo.Services;

/// <summary>
/// SINGLE RESPONSIBILITY PRINCIPLE (SRP) DEMONSTRATION
/// 
/// Each service class has a single, well-defined responsibility.
/// This makes the code easier to understand, test, and maintain.
/// Changes to one responsibility don't affect other classes.
/// </summary>

/// <summary>
/// Responsible ONLY for validating orders - SRP compliance
/// </summary>
public class OrderValidationService : IOrderValidator
{
    private readonly IInventoryChecker _inventoryChecker;

    public OrderValidationService(IInventoryChecker inventoryChecker)
    {
        _inventoryChecker = inventoryChecker;
    }

    public bool ValidateOrder(Order order)
    {
        return GetValidationErrors(order).Count() == 0;
    }

    public IEnumerable<string> GetValidationErrors(Order order)
    {
        var errors = new List<string>();

        // Basic order validation
        if (order.Items.Count == 0)
            errors.Add("Order must contain at least one item");

        if (order.Subtotal <= 0)
            errors.Add("Order subtotal must be greater than zero");

        // Inventory validation
        foreach (var item in order.Items)
        {
            if (!_inventoryChecker.IsInStock(item.ProductId, item.Quantity))
                errors.Add($"Product {item.ProductName} is not available in requested quantity");
        }

        // Customer validation
        if (string.IsNullOrWhiteSpace(order.CustomerId))
            errors.Add("Customer ID is required");

        return errors;
    }
}

/// <summary>
/// Responsible ONLY for sending notifications - SRP compliance
/// </summary>
public class NotificationService : INotificationSender
{
    private readonly List<INotificationChannel> _channels;

    public NotificationService(IEnumerable<INotificationChannel> channels)
    {
        _channels = channels.ToList();
    }

    public void SendOrderConfirmation(Order order)
    {
        var message = $"Order {order.OrderId} confirmed. Total: {order.TotalAmount:C}";
        SendNotification(message, NotificationType.OrderConfirmation);
    }

    public void SendShippingNotification(Order order, string trackingNumber)
    {
        var message = $"Order {order.OrderId} shipped. Tracking: {trackingNumber}";
        SendNotification(message, NotificationType.Shipping);
    }

    public void SendOrderCancellation(Order order)
    {
        var message = $"Order {order.OrderId} has been cancelled.";
        SendNotification(message, NotificationType.Cancellation);
    }

    private void SendNotification(string message, NotificationType type)
    {
        foreach (var channel in _channels)
        {
            channel.SendNotification(message, type);
        }
    }
}

/// <summary>
/// Responsible ONLY for inventory management - SRP compliance
/// </summary>
public class InventoryService : IInventoryChecker
{
    private readonly Dictionary<string, int> _inventory;

    public InventoryService()
    {
        // Initialize with some sample inventory
        _inventory = new Dictionary<string, int>
        {
            { "LAPTOP-001", 50 },
            { "PHONE-002", 100 },
            { "SOFTWARE-003", 999 }, // Digital products have high inventory
            { "SUBSCRIPTION-004", 999 }
        };
    }

    public bool IsInStock(string productId, int quantity)
    {
        return _inventory.GetValueOrDefault(productId, 0) >= quantity;
    }

    public void ReserveInventory(string productId, int quantity)
    {
        if (!IsInStock(productId, quantity))
            throw new InvalidOperationException($"Insufficient inventory for product {productId}");

        _inventory[productId] -= quantity;
    }

    public void ReleaseInventory(string productId, int quantity)
    {
        if (_inventory.ContainsKey(productId))
        {
            _inventory[productId] += quantity;
        }
        else
        {
            _inventory[productId] = quantity;
        }
    }
}

/// <summary>
/// Responsible ONLY for payment processing - SRP compliance
/// </summary>
public class PaymentService : IPaymentProcessor
{
    private readonly Dictionary<string, decimal> _processedPayments;

    public PaymentService()
    {
        _processedPayments = new Dictionary<string, decimal>();
    }

    public bool ProcessPayment(Order order, string paymentMethod)
    {
        // Simulate payment processing logic
        if (string.IsNullOrWhiteSpace(paymentMethod))
            return false;

        if (order.TotalAmount <= 0)
            return false;

        // Simulate different payment methods having different success rates
        var success = paymentMethod.ToLower() switch
        {
            "creditcard" => true,
            "paypal" => true,
            "banktransfer" => order.TotalAmount <= 10000, // Large amounts might fail
            _ => false
        };

        if (success)
        {
            _processedPayments[order.OrderId] = order.TotalAmount;
        }

        return success;
    }

    public void RefundPayment(string orderId, decimal amount)
    {
        if (_processedPayments.ContainsKey(orderId))
        {
            var processedAmount = _processedPayments[orderId];
            if (amount <= processedAmount)
            {
                _processedPayments[orderId] -= amount;
                // Log refund processing
                Console.WriteLine($"Refunded {amount:C} for order {orderId}");
            }
        }
    }
}

/// <summary>
/// Notification channel interface for the Strategy pattern
/// </summary>
public interface INotificationChannel
{
    void SendNotification(string message, NotificationType type);
    string GetChannelName();
}

public enum NotificationType
{
    OrderConfirmation,
    Shipping,
    Cancellation,
    PaymentIssue
}

/// <summary>
/// Email notification implementation
/// </summary>
public class EmailNotificationChannel : INotificationChannel
{
    public void SendNotification(string message, NotificationType type)
    {
        Console.WriteLine($"[EMAIL] {type}: {message}");
    }

    public string GetChannelName() => "Email";
}

/// <summary>
/// SMS notification implementation
/// </summary>
public class SmsNotificationChannel : INotificationChannel
{
    public void SendNotification(string message, NotificationType type)
    {
        Console.WriteLine($"[SMS] {type}: {message}");
    }

    public string GetChannelName() => "SMS";
}

/// <summary>
/// Push notification implementation
/// </summary>
public class PushNotificationChannel : INotificationChannel
{
    public void SendNotification(string message, NotificationType type)
    {
        Console.WriteLine($"[PUSH] {type}: {message}");
    }

    public string GetChannelName() => "Push Notification";
}