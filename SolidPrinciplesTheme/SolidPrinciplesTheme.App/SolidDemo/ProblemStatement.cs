namespace SolidPrinciplesTheme.App.SolidDemo;

/// <summary>
/// SOLID PRINCIPLES DEMONSTRATION - E-COMMERCE ORDER PROCESSING
/// 
/// PROBLEM: A growing e-commerce platform needs to process orders with complex requirements:
/// 1. Multiple order types (physical products, digital downloads, subscription services)
/// 2. Different pricing strategies (regular, bulk discount, membership discount, seasonal)
/// 3. Various notification methods (email, SMS, push notifications)
/// 4. Multiple payment processors (PayPal, Stripe, bank transfer)
/// 5. Different shipping strategies (standard, express, international)
/// 6. Order validation rules (inventory check, fraud detection, age verification)
/// 
/// CHALLENGE: Create a flexible, maintainable system that can easily accommodate:
/// - New order types without modifying existing code
/// - New pricing strategies
/// - New notification channels
/// - New payment methods
/// - New shipping options
/// - New validation rules
/// 
/// SOLUTION: Apply SOLID principles to create a robust, extensible architecture.
/// </summary>
public static class ProblemStatement
{
    public const string Description = @"
E-COMMERCE ORDER PROCESSING SYSTEM

This system demonstrates all SOLID principles through a single, cohesive problem domain:
processing e-commerce orders with varying requirements for different order types.

SOLID PRINCIPLES APPLIED:
1. Single Responsibility - Each class has one reason to change
2. Open/Closed - Open for extension, closed for modification
3. Liskov Substitution - Derived classes can replace base classes seamlessly
4. Interface Segregation - Clients depend only on interfaces they use
5. Dependency Inversion - High-level modules don't depend on low-level modules
";
}