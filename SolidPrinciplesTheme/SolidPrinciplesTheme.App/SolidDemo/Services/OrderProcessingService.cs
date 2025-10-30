using SolidPrinciplesTheme.App.SolidDemo.Models;
using SolidPrinciplesTheme.App.SolidDemo.Interfaces;
using SolidPrinciplesTheme.App.SolidDemo.Strategies;

namespace SolidPrinciplesTheme.App.SolidDemo.Services;

/// <summary>
/// DEPENDENCY INVERSION PRINCIPLE (DIP) DEMONSTRATION
/// 
/// This high-level module (OrderProcessingService) does NOT depend on low-level modules.
/// Instead, it depends on abstractions (interfaces).
/// Low-level modules also depend on the same abstractions.
/// This allows for flexible, testable, and maintainable code.
/// </summary>
public class OrderProcessingService
{
    // All dependencies are abstractions (interfaces), not concrete implementations
    private readonly IOrderValidator _orderValidator;
    private readonly IOrderWriter _orderWriter;
    private readonly IOrderReader _orderReader;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly INotificationSender _notificationSender;
    private readonly IInventoryChecker _inventoryChecker;
    private readonly IPricingStrategy _pricingStrategy;

    /// <summary>
    /// Constructor injection of dependencies - all are abstractions, not concrete classes.
    /// This demonstrates DIP - we depend on abstractions, not concretions.
    /// </summary>
    public OrderProcessingService(
        IOrderValidator orderValidator,
        IOrderWriter orderWriter,
        IOrderReader orderReader,
        IPaymentProcessor paymentProcessor,
        INotificationSender notificationSender,
        IInventoryChecker inventoryChecker,
        IPricingStrategy pricingStrategy)
    {
        _orderValidator = orderValidator ?? throw new ArgumentNullException(nameof(orderValidator));
        _orderWriter = orderWriter ?? throw new ArgumentNullException(nameof(orderWriter));
        _orderReader = orderReader ?? throw new ArgumentNullException(nameof(orderReader));
        _paymentProcessor = paymentProcessor ?? throw new ArgumentNullException(nameof(paymentProcessor));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        _inventoryChecker = inventoryChecker ?? throw new ArgumentNullException(nameof(inventoryChecker));
        _pricingStrategy = pricingStrategy ?? throw new ArgumentNullException(nameof(pricingStrategy));
    }

    /// <summary>
    /// Main business logic method that orchestrates the entire order processing workflow.
    /// This method demonstrates how high-level business logic can remain stable
    /// while low-level implementation details can vary.
    /// </summary>
    public async Task<OrderProcessingResult> ProcessOrderAsync(Order order, string paymentMethod)
    {
        var result = new OrderProcessingResult { OrderId = order.OrderId };

        try
        {
            // Step 1: Calculate final price using injected pricing strategy
            var finalPrice = _pricingStrategy.CalculatePrice(order);
            order.SetTotalAmount(finalPrice);
            result.TotalAmount = finalPrice;
            result.PricingStrategy = _pricingStrategy.GetStrategyName();

            // Step 2: Validate order using injected validator
            if (!_orderValidator.ValidateOrder(order))
            {
                var errors = _orderValidator.GetValidationErrors(order);
                result.IsSuccess = false;
                result.ErrorMessage = $"Validation failed: {string.Join(", ", errors)}";
                return result;
            }

            // Step 3: Reserve inventory using injected inventory checker
            foreach (var item in order.Items)
            {
                _inventoryChecker.ReserveInventory(item.ProductId, item.Quantity);
            }

            // Step 4: Process payment using injected payment processor
            order.SetPaymentMethod(paymentMethod);
            var paymentSuccess = _paymentProcessor.ProcessPayment(order, paymentMethod);
            
            if (!paymentSuccess)
            {
                // Release reserved inventory if payment fails
                foreach (var item in order.Items)
                {
                    _inventoryChecker.ReleaseInventory(item.ProductId, item.Quantity);
                }

                result.IsSuccess = false;
                result.ErrorMessage = "Payment processing failed";
                return result;
            }

            // Step 5: Update order status and save using injected repository
            order.UpdateStatus(OrderStatus.PaymentProcessed);
            _orderWriter.SaveOrder(order);

            // Step 6: Send confirmation notification using injected notification service
            _notificationSender.SendOrderConfirmation(order);

            result.IsSuccess = true;
            result.Message = $"Order {order.OrderId} processed successfully";

            return result;
        }
        catch (Exception ex)
        {
            // If anything fails, try to release inventory and update order status
            try
            {
                foreach (var item in order.Items)
                {
                    _inventoryChecker.ReleaseInventory(item.ProductId, item.Quantity);
                }
                
                order.UpdateStatus(OrderStatus.Cancelled);
                _orderWriter.SaveOrder(order);
            }
            catch
            {
                // Log the cleanup failure, but don't throw
            }

            result.IsSuccess = false;
            result.ErrorMessage = $"Order processing failed: {ex.Message}";
            return result;
        }
    }

    /// <summary>
    /// Demonstrates how the service can work with different order types
    /// while maintaining the same interface (Liskov Substitution Principle)
    /// </summary>
    public string GetOrderTypeSpecificInstructions(OrderType orderType, Order order)
    {
        // The orderType parameter can be any derived class (PhysicalProductOrder, 
        // DigitalProductOrder, SubscriptionOrder) because they all properly 
        // implement the base class contract (LSP compliance)
        
        var processingTime = orderType.GetProcessingTime();
        var isValid = orderType.ValidateOrder(order);
        var instructions = orderType.GetFulfillmentInstructions();

        return $"Order Type: {orderType.Name}\n" +
               $"Processing Time: {processingTime}\n" +
               $"Valid: {isValid}\n" +
               $"Instructions: {instructions}";
    }

    /// <summary>
    /// Get order status - demonstrates read-only operations
    /// </summary>
    public Order? GetOrder(string orderId)
    {
        return _orderReader.GetOrderById(orderId);
    }

    /// <summary>
    /// Cancel an order - demonstrates the full workflow with rollback
    /// </summary>
    public async Task<bool> CancelOrderAsync(string orderId)
    {
        var order = _orderReader.GetOrderById(orderId);
        if (order == null) return false;

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            return false;

        try
        {
            // Release inventory
            foreach (var item in order.Items)
            {
                _inventoryChecker.ReleaseInventory(item.ProductId, item.Quantity);
            }

            // Process refund if payment was processed
            if (order.Status == OrderStatus.PaymentProcessed || order.Status == OrderStatus.Shipped)
            {
                _paymentProcessor.RefundPayment(order.OrderId, order.TotalAmount);
            }

            // Update status and save
            order.UpdateStatus(OrderStatus.Cancelled);
            _orderWriter.SaveOrder(order);

            // Send notification
            _notificationSender.SendOrderCancellation(order);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Result object for order processing operations
/// </summary>
public class OrderProcessingResult
{
    public string OrderId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PricingStrategy { get; set; } = string.Empty;
}