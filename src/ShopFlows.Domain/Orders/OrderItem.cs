using ShopFlows.Domain.Common;

namespace ShopFlows.Domain.Orders;

/// <summary>
/// A line inside an <see cref="Order"/>. It can only be created and changed through
/// its Order, so the aggregate rules cannot be bypassed.
/// </summary>
public sealed class OrderItem : Entity
{
    internal OrderItem(Guid productId, string productName, Money unitPrice, int quantity)
        : base(Guid.CreateVersion7())
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; }

    public string ProductName { get; }

    /// <summary>Price captured when the item was added; later catalog price changes do not affect it.</summary>
    public Money UnitPrice { get; }

    public int Quantity { get; private set; }

    public Money LineTotal => UnitPrice.Multiply(Quantity);

    internal void ChangeQuantity(int quantity) => Quantity = quantity;
}
