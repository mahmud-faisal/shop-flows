using FluentAssertions;
using ShopFlows.Domain.Catalog;
using ShopFlows.Domain.Common;
using ShopFlows.Domain.Orders;

namespace ShopFlows.UnitTests.Domain.Orders;

public class OrderTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    private static Product NewProduct(decimal price = 10m, string currency = "USD", string name = "Widget") =>
        new($"SKU-{Guid.NewGuid():N}", name, new Money(price, currency), CategoryId);

    private static Order NewOrder(string currency = "USD") => new(Guid.NewGuid(), currency);

    [Fact]
    public void New_order_is_empty_draft_with_zero_total()
    {
        var order = NewOrder();

        order.Status.Should().Be(OrderStatus.Draft);
        order.Items.Should().BeEmpty();
        order.Total.Should().Be(Money.Zero("USD"));
    }

    [Fact]
    public void Constructor_rejects_empty_customer()
    {
        var act = () => new Order(Guid.Empty, "USD");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_adds_line_and_updates_total()
    {
        var order = NewOrder();
        var product = NewProduct(price: 12.50m);

        order.AddItem(product, 2);

        order.Items.Should().ContainSingle();
        var item = order.Items.Single();
        item.ProductId.Should().Be(product.Id);
        item.ProductName.Should().Be("Widget");
        item.UnitPrice.Should().Be(new Money(12.50m, "USD"));
        item.Quantity.Should().Be(2);
        order.Total.Should().Be(new Money(25m, "USD"));
    }

    [Fact]
    public void AddItem_merges_same_product_into_one_line()
    {
        var order = NewOrder();
        var product = NewProduct();

        order.AddItem(product, 2);
        order.AddItem(product, 3);

        order.Items.Should().ContainSingle().Which.Quantity.Should().Be(5);
    }

    [Fact]
    public void Total_sums_all_lines()
    {
        var order = NewOrder();

        order.AddItem(NewProduct(price: 10m), 2);
        order.AddItem(NewProduct(price: 2.50m), 4);

        order.Total.Should().Be(new Money(30m, "USD"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_rejects_non_positive_quantity(int quantity)
    {
        var order = NewOrder();

        var act = () => order.AddItem(NewProduct(), quantity);

        act.Should().Throw<DomainException>();
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_rejects_quantity_above_line_limit()
    {
        var order = NewOrder();

        var act = () => order.AddItem(NewProduct(), Order.MaxQuantityPerLine + 1);

        act.Should().Throw<DomainException>().WithMessage("*cannot exceed*");
    }

    [Fact]
    public void AddItem_rejects_merge_that_exceeds_line_limit()
    {
        var order = NewOrder();
        var product = NewProduct();
        order.AddItem(product, Order.MaxQuantityPerLine);

        var act = () => order.AddItem(product, 1);

        act.Should().Throw<DomainException>();
        order.Items.Single().Quantity.Should().Be(Order.MaxQuantityPerLine);
    }

    [Fact]
    public void AddItem_rejects_inactive_product()
    {
        var order = NewOrder();
        var product = NewProduct();
        product.Deactivate();

        var act = () => order.AddItem(product, 1);

        act.Should().Throw<DomainException>().WithMessage("*not available*");
    }

    [Fact]
    public void AddItem_rejects_product_in_other_currency()
    {
        var order = NewOrder("USD");

        var act = () => order.AddItem(NewProduct(currency: "EUR"), 1);

        act.Should().Throw<DomainException>().WithMessage("*EUR*USD*");
    }

    [Fact]
    public void Item_keeps_price_captured_when_added()
    {
        var order = NewOrder();
        var product = NewProduct(price: 10m);
        order.AddItem(product, 1);

        product.ChangePrice(new Money(99m, "USD"));

        order.Total.Should().Be(new Money(10m, "USD"));
    }

    [Fact]
    public void RemoveItem_removes_existing_line()
    {
        var order = NewOrder();
        var keep = NewProduct();
        var remove = NewProduct();
        order.AddItem(keep, 1);
        order.AddItem(remove, 1);

        order.RemoveItem(remove.Id);

        order.Items.Should().ContainSingle().Which.ProductId.Should().Be(keep.Id);
    }

    [Fact]
    public void RemoveItem_rejects_unknown_product()
    {
        var order = NewOrder();

        var act = () => order.RemoveItem(Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("*does not contain*");
    }

    [Fact]
    public void ChangeItemQuantity_updates_quantity()
    {
        var order = NewOrder();
        var product = NewProduct(price: 5m);
        order.AddItem(product, 1);

        order.ChangeItemQuantity(product.Id, 4);

        order.Items.Single().Quantity.Should().Be(4);
        order.Total.Should().Be(new Money(20m, "USD"));
    }

    [Fact]
    public void ChangeItemQuantity_to_zero_removes_line()
    {
        var order = NewOrder();
        var product = NewProduct();
        order.AddItem(product, 3);

        order.ChangeItemQuantity(product.Id, 0);

        order.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(Order.MaxQuantityPerLine + 1)]
    public void ChangeItemQuantity_rejects_out_of_range_quantity(int quantity)
    {
        var order = NewOrder();
        var product = NewProduct();
        order.AddItem(product, 1);

        var act = () => order.ChangeItemQuantity(product.Id, quantity);

        act.Should().Throw<DomainException>();
        order.Items.Single().Quantity.Should().Be(1);
    }

    [Fact]
    public void Place_rejects_empty_order()
    {
        var act = () => NewOrder().Place();

        act.Should().Throw<DomainException>().WithMessage("*empty*");
    }

    [Fact]
    public void Place_moves_order_to_placed()
    {
        var order = NewOrder();
        order.AddItem(NewProduct(), 1);

        order.Place();

        order.Status.Should().Be(OrderStatus.Placed);
    }

    [Fact]
    public void Placed_order_cannot_be_changed()
    {
        var order = NewOrder();
        var product = NewProduct();
        order.AddItem(product, 1);
        order.Place();

        order.Invoking(o => o.AddItem(NewProduct(), 1)).Should().Throw<DomainException>();
        order.Invoking(o => o.RemoveItem(product.Id)).Should().Throw<DomainException>();
        order.Invoking(o => o.ChangeItemQuantity(product.Id, 2)).Should().Throw<DomainException>();
        order.Invoking(o => o.Place()).Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_works_from_draft_and_placed()
    {
        var draft = NewOrder();
        var placed = NewOrder();
        placed.AddItem(NewProduct(), 1);
        placed.Place();

        draft.Cancel();
        placed.Cancel();

        draft.Status.Should().Be(OrderStatus.Cancelled);
        placed.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_rejects_already_cancelled_order()
    {
        var order = NewOrder();
        order.Cancel();

        var act = order.Cancel;

        act.Should().Throw<DomainException>().WithMessage("*already cancelled*");
    }

    [Fact]
    public void Cancelled_order_cannot_be_changed()
    {
        var order = NewOrder();
        order.Cancel();

        order.Invoking(o => o.AddItem(NewProduct(), 1)).Should().Throw<DomainException>();
    }

    [Fact]
    public void Items_cannot_be_modified_from_outside()
    {
        var order = NewOrder();
        order.AddItem(NewProduct(), 1);

        order.Items.Should().BeAssignableTo<IReadOnlyCollection<OrderItem>>();
        order.Items.Should().NotBeAssignableTo<List<OrderItem>>();
    }
}
