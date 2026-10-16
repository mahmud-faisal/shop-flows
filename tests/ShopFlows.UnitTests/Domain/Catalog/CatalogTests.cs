using FluentAssertions;
using ShopFlows.Domain.Catalog;
using ShopFlows.Domain.Common;

namespace ShopFlows.UnitTests.Domain.Catalog;

public class CatalogTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void Product_is_created_active_with_normalized_sku()
    {
        var product = new Product(" abc-1 ", "Mug", new Money(8m, "USD"), CategoryId);

        product.Sku.Should().Be("ABC-1");
        product.IsActive.Should().BeTrue();
        product.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Product_rejects_zero_price()
    {
        var act = () => new Product("A", "Mug", Money.Zero("USD"), CategoryId);

        act.Should().Throw<DomainException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Product_rejects_missing_category()
    {
        var act = () => new Product("A", "Mug", new Money(1m, "USD"), Guid.Empty);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangePrice_updates_price_in_same_currency()
    {
        var product = new Product("A", "Mug", new Money(8m, "USD"), CategoryId);

        product.ChangePrice(new Money(9m, "USD"));

        product.Price.Should().Be(new Money(9m, "USD"));
    }

    [Fact]
    public void ChangePrice_rejects_currency_switch()
    {
        var product = new Product("A", "Mug", new Money(8m, "USD"), CategoryId);

        var act = () => product.ChangePrice(new Money(9m, "EUR"));

        act.Should().Throw<DomainException>();
        product.Price.Currency.Should().Be("USD");
    }

    [Fact]
    public void Deactivate_and_activate_toggle_availability()
    {
        var product = new Product("A", "Mug", new Money(8m, "USD"), CategoryId);

        product.Deactivate();
        product.IsActive.Should().BeFalse();

        product.Activate();
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Category_rename_trims_and_rejects_blank()
    {
        var category = new Category("Kitchen");

        category.Rename("  Home  ");
        category.Name.Should().Be("Home");

        category.Invoking(c => c.Rename(" ")).Should().Throw<DomainException>();
    }
}
