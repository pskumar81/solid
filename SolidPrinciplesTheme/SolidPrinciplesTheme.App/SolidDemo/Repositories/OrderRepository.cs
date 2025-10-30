using SolidPrinciplesTheme.App.SolidDemo.Models;
using SolidPrinciplesTheme.App.SolidDemo.Interfaces;

namespace SolidPrinciplesTheme.App.SolidDemo.Repositories;

/// <summary>
/// Repository implementation demonstrating Interface Segregation Principle.
/// This class implements multiple small, focused interfaces rather than one large interface.
/// Each interface serves a specific purpose and clients depend only on what they need.
/// </summary>
public class InMemoryOrderRepository : IOrderReader, IOrderWriter
{
    private readonly Dictionary<string, Order> _orders;
    private readonly object _lock = new object();

    public InMemoryOrderRepository()
    {
        _orders = new Dictionary<string, Order>();
    }

    #region IOrderReader Implementation

    public Order? GetOrderById(string orderId)
    {
        lock (_lock)
        {
            return _orders.GetValueOrDefault(orderId);
        }
    }

    public IEnumerable<Order> GetOrdersByCustomer(string customerId)
    {
        lock (_lock)
        {
            return _orders.Values
                .Where(order => order.CustomerId == customerId)
                .ToList();
        }
    }

    public IEnumerable<Order> GetOrdersByStatus(OrderStatus status)
    {
        lock (_lock)
        {
            return _orders.Values
                .Where(order => order.Status == status)
                .ToList();
        }
    }

    #endregion

    #region IOrderWriter Implementation

    public void SaveOrder(Order order)
    {
        lock (_lock)
        {
            _orders[order.OrderId] = order;
        }
    }

    public void UpdateOrderStatus(string orderId, OrderStatus status)
    {
        lock (_lock)
        {
            if (_orders.TryGetValue(orderId, out var order))
            {
                order.UpdateStatus(status);
            }
        }
    }

    public void DeleteOrder(string orderId)
    {
        lock (_lock)
        {
            _orders.Remove(orderId);
        }
    }

    #endregion

    // Additional helper method for demo purposes
    public int GetOrderCount()
    {
        lock (_lock)
        {
            return _orders.Count;
        }
    }

    public IEnumerable<Order> GetAllOrders()
    {
        lock (_lock)
        {
            return _orders.Values.ToList();
        }
    }
}