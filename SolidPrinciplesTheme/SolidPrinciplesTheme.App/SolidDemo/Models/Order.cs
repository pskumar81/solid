namespace SolidPrinciplesTheme.App.SolidDemo.Models;

/// <summary>
/// Core domain model representing an order in the e-commerce system.
/// This class follows SRP by focusing solely on representing order data.
/// </summary>
public class Order
{
    public Order(string orderId, string customerId, decimal subtotal, DateTime orderDate)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID cannot be null or empty", nameof(orderId));
        
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be null or empty", nameof(customerId));
        
        if (subtotal < 0)
            throw new ArgumentException("Subtotal cannot be negative", nameof(subtotal));

        OrderId = orderId;
        CustomerId = customerId;
        Subtotal = subtotal;
        OrderDate = orderDate;
        Items = new List<OrderItem>();
        Status = OrderStatus.Pending;
    }

    public string OrderId { get; }
    public string CustomerId { get; }
    public decimal Subtotal { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime OrderDate { get; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; }
    public string? ShippingAddress { get; private set; }
    public string? PaymentMethod { get; private set; }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
        RecalculateSubtotal();
    }

    public void SetTotalAmount(decimal totalAmount)
    {
        TotalAmount = totalAmount;
    }

    public void SetShippingAddress(string address)
    {
        ShippingAddress = address;
    }

    public void SetPaymentMethod(string paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }

    private void RecalculateSubtotal()
    {
        Subtotal = Items.Sum(item => item.Price * item.Quantity);
    }
}

public class OrderItem
{
    public OrderItem(string productId, string productName, decimal price, int quantity, ProductType productType)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
        ProductType = productType;
    }

    public string ProductId { get; }
    public string ProductName { get; }
    public decimal Price { get; }
    public int Quantity { get; }
    public ProductType ProductType { get; }
}

public enum OrderStatus
{
    Pending,
    Validated,
    PaymentProcessed,
    Shipped,
    Delivered,
    Cancelled
}

public enum ProductType
{
    Physical,
    Digital,
    Subscription
}