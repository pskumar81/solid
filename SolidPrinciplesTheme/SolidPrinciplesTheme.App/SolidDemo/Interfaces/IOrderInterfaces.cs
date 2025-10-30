using SolidPrinciplesTheme.App.SolidDemo.Models;

namespace SolidPrinciplesTheme.App.SolidDemo.Interfaces;

/// <summary>
/// INTERFACE SEGREGATION PRINCIPLE (ISP) DEMONSTRATION
/// 
/// Instead of one large interface, we create multiple small, focused interfaces.
/// Clients depend only on the interfaces they actually use.
/// This prevents implementing unnecessary methods and reduces coupling.
/// </summary>

/// <summary>
/// Interface for reading order data - used by reporting services
/// </summary>
public interface IOrderReader
{
    Order? GetOrderById(string orderId);
    IEnumerable<Order> GetOrdersByCustomer(string customerId);
    IEnumerable<Order> GetOrdersByStatus(OrderStatus status);
}

/// <summary>
/// Interface for writing order data - used by order processing services
/// </summary>
public interface IOrderWriter
{
    void SaveOrder(Order order);
    void UpdateOrderStatus(string orderId, OrderStatus status);
    void DeleteOrder(string orderId);
}

/// <summary>
/// Interface for order validation - used by validation services
/// </summary>
public interface IOrderValidator
{
    bool ValidateOrder(Order order);
    IEnumerable<string> GetValidationErrors(Order order);
}

/// <summary>
/// Interface for inventory checking - used by inventory services
/// </summary>
public interface IInventoryChecker
{
    bool IsInStock(string productId, int quantity);
    void ReserveInventory(string productId, int quantity);
    void ReleaseInventory(string productId, int quantity);
}

/// <summary>
/// Interface for payment processing - used by payment services
/// </summary>
public interface IPaymentProcessor
{
    bool ProcessPayment(Order order, string paymentMethod);
    void RefundPayment(string orderId, decimal amount);
}

/// <summary>
/// Interface for notifications - used by notification services
/// </summary>
public interface INotificationSender
{
    void SendOrderConfirmation(Order order);
    void SendShippingNotification(Order order, string trackingNumber);
    void SendOrderCancellation(Order order);
}

/// <summary>
/// Interface for shipping - used by shipping services
/// </summary>
public interface IShippingProvider
{
    string CalculateShippingCost(Order order, string shippingMethod);
    string CreateShipment(Order order, string shippingMethod);
    string GetTrackingNumber(string shipmentId);
}