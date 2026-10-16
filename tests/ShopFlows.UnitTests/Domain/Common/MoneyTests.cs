using FluentAssertions;
using ShopFlows.Domain.Common;

namespace ShopFlows.UnitTests.Domain.Common;

public class MoneyTests
{
    [Fact]
    public void Constructor_normalizes_currency_code()
    {
        var money = new Money(10m, " usd ");

        money.Currency.Should().Be("USD");
        money.Amount.Should().Be(10m);
    }

    [Fact]
    public void Constructor_rejects_negative_amount()
    {
        var act = () => new Money(-0.01m, "USD");

        act.Should().Throw<DomainException>().WithMessage("*negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("USDT")]
    [InlineData("U5D")]
    public void Constructor_rejects_invalid_currency(string currency)
    {
        var act = () => new Money(1m, currency);

        act.Should().Throw<DomainException>().WithMessage("*ISO*");
    }

    [Fact]
    public void Zero_creates_zero_amount_in_currency()
    {
        Money.Zero("EUR").Should().Be(new Money(0m, "EUR"));
    }

    [Fact]
    public void Add_sums_amounts_with_same_currency()
    {
        var result = new Money(10.50m, "USD").Add(new Money(4.25m, "USD"));

        result.Should().Be(new Money(14.75m, "USD"));
    }

    [Fact]
    public void Add_rejects_different_currency()
    {
        var act = () => new Money(10m, "USD").Add(new Money(5m, "EUR"));

        act.Should().Throw<DomainException>().WithMessage("Currency mismatch*");
    }

    [Fact]
    public void Subtract_returns_difference_with_same_currency()
    {
        var result = new Money(10m, "USD").Subtract(new Money(3m, "USD"));

        result.Should().Be(new Money(7m, "USD"));
    }

    [Fact]
    public void Subtract_rejects_different_currency()
    {
        var act = () => new Money(10m, "USD").Subtract(new Money(3m, "GBP"));

        act.Should().Throw<DomainException>().WithMessage("Currency mismatch*");
    }

    [Fact]
    public void Subtract_rejects_negative_result()
    {
        var act = () => new Money(3m, "USD").Subtract(new Money(10m, "USD"));

        act.Should().Throw<DomainException>().WithMessage("*negative*");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 2.5)]
    [InlineData(3, 7.5)]
    public void Multiply_scales_amount_by_quantity(int quantity, decimal expected)
    {
        var result = new Money(2.50m, "USD").Multiply(quantity);

        result.Should().Be(new Money(expected, "USD"));
    }

    [Fact]
    public void Multiply_rejects_negative_quantity()
    {
        var act = () => new Money(2m, "USD").Multiply(-1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Operations_do_not_mutate_the_original()
    {
        var original = new Money(10m, "USD");

        _ = original.Add(new Money(5m, "USD"));
        _ = original.Multiply(3);

        original.Amount.Should().Be(10m);
    }

    [Fact]
    public void Equality_is_by_value()
    {
        var a = new Money(9.99m, "USD");
        var b = new Money(9.99m, "usd");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Should().NotBe(new Money(9.99m, "EUR"));
    }

    [Fact]
    public void ToString_shows_amount_and_currency()
    {
        new Money(5m, "USD").ToString().Should().Be("5.00 USD");
    }
}
