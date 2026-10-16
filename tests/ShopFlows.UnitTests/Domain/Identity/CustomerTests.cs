using FluentAssertions;
using ShopFlows.Domain.Common;
using ShopFlows.Domain.Identity;

namespace ShopFlows.UnitTests.Domain.Identity;

public class CustomerTests
{
    [Fact]
    public void Customer_normalizes_email()
    {
        var customer = new Customer("Ada", "  Ada@Example.COM ");

        customer.Email.Should().Be("ada@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("no-at-sign")]
    [InlineData("@example.com")]
    [InlineData("a@b")]
    [InlineData("a@@example.com")]
    [InlineData("a b@example.com")]
    [InlineData("a@example.")]
    public void Customer_rejects_invalid_email(string email)
    {
        var act = () => new Customer("Ada", email);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangeEmail_replaces_valid_email_and_keeps_old_on_failure()
    {
        var customer = new Customer("Ada", "ada@example.com");

        customer.ChangeEmail("ada@shopflows.dev");
        customer.Email.Should().Be("ada@shopflows.dev");

        customer.Invoking(c => c.ChangeEmail("broken")).Should().Throw<DomainException>();
        customer.Email.Should().Be("ada@shopflows.dev");
    }

    [Fact]
    public void Entities_are_equal_only_by_identity()
    {
        var a = new Customer("Ada", "ada@example.com");
        var b = new Customer("Ada", "ada@example.com");

        a.Should().NotBe(b);
        a.Should().Be(a);
    }
}
