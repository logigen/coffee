using System;

public enum PaymentType
{
    Cash,
    Card
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int ClientId { get; set; }
    public string ItemsJson { get; set; }
    public OrderStatus Status { get; set; }
    public Address DeliveryAddress { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentType { get; set; }
    public bool IsPaid { get; set; }
    public int? CourierId { get; set; }
    public string DeliveryInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}