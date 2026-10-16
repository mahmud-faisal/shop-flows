// Pattern: Aggregate (aggregate root)
// Problem it solves here: an order and its lines must stay consistent. Without a single
//   gatekeeper, any code could add a line in another currency, push a quantity to 500,
//   or edit lines after the order was placed.
// Why this pattern: Order is the only entry point for changing its OrderItems. Every rule
//   (draft-only edits, quantity limits, one currency, active products only) is checked in
//   one place, so an Order is always valid after any public method returns.
// When NOT to use it: for simple lookup data with no invariants spanning several objects
//   (a Category), or when you are tempted to pull unrelated entities (Customer, Product)
//   inside the boundary just because they are related. Reference those by Id instead.

using ShopFlows.Domain.Catalog;
using ShopFlows.Domain.Common;

namespace ShopFlows.Domain.Orders;

public sealed class Order : Entity
{
    public const int MaxQuantityPerLine = 99;

    private readonly List<OrderItem> _items = [];

    public Order(Guid customerId, string currency)
        : base(Guid.CreateVersion7())
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Order must belong to a customer.");

        CustomerId = customerId;
        Currency = Money.Zero(currency).Currency;
        Status = OrderStatus.Draft;
    }

    public Guid CustomerId { get; }

    public string Currency { get; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money Total => _items.Aggregate(Money.Zero(Currency), (sum, item) => sum.Add(item.LineTotal));

    public void AddItem(Product product, int quantity)
    {
        EnsureDraft();
        EnsurePositive(quantity);

        if (!product.IsActive)
            throw new DomainException($"Product '{product.Name}' is not available.");
        if (product.Price.Currency != Currency)
            throw new DomainException($"Product is priced in {product.Price.Currency} but the order uses {Currency}.");

        var existing = FindItem(product.Id);
        if (existing is not null)
        {
            var merged = existing.Quantity + quantity;
            EnsureWithinLimit(merged);
            existing.ChangeQuantity(merged);
            return;
        }

        EnsureWithinLimit(quantity);
        _items.Add(new OrderItem(product.Id, product.Name, product.Price, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        EnsureDraft();

        var item = FindItem(productId)
            ?? throw new DomainException("The order does not contain this product.");

        _items.Remove(item);
    }

    public void ChangeItemQuantity(Guid productId, int quantity)
    {
        EnsureDraft();

        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative.");

        var item = FindItem(productId)
            ?? throw new DomainException("The order does not contain this product.");

        if (quantity == 0)
        {
            _items.Remove(item);
            return;
        }

        EnsureWithinLimit(quantity);
        item.ChangeQuantity(quantity);
    }

    public void Place()
    {
        EnsureDraft();

        if (_items.Count == 0)
            throw new DomainException("Cannot place an empty order.");

        Status = OrderStatus.Placed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled.");

        Status = OrderStatus.Cancelled;
    }

    private OrderItem? FindItem(Guid productId) => _items.Find(i => i.ProductId == productId);

    private void EnsureDraft()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException($"Order cannot be changed while it is {Status}.");
    }

    private static void EnsurePositive(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");
    }

    private static void EnsureWithinLimit(int quantity)
    {
        if (quantity > MaxQuantityPerLine)
            throw new DomainException($"A single line cannot exceed {MaxQuantityPerLine} units.");
    }
}
