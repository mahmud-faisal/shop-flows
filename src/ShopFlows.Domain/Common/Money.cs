// Pattern: Value Object
// Problem it solves here: prices and totals were about to travel as a bare decimal plus a
//   separate currency string. Nothing would stop code from adding USD to EUR, creating a
//   negative price, or forgetting the currency altogether.
// Why this pattern: Money is defined only by its amount and currency, has no identity, and
//   never changes after creation. Equality by value, validation in the constructor, and
//   arithmetic that checks currency keep every money calculation in ShopFlows correct by
//   construction.
// When NOT to use it: when the concept has its own lifecycle and identity (a Customer or an
//   Order), or when the value is a plain number with no rules attached (a page size).

namespace ShopFlows.Domain.Common;

/// <summary>
/// An immutable, non-negative amount of money in a single ISO 4217 currency.
/// </summary>
public sealed record Money
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        Currency = NormalizeCurrency(currency);
        Amount = amount;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public static Money Zero(string currency) => new(0m, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (other.Amount > Amount)
            throw new DomainException("Subtraction would produce a negative amount.");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative.");

        return new Money(Amount * quantity, Currency);
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";

    private void EnsureSameCurrency(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException($"Currency mismatch: {Currency} vs {other.Currency}.");
    }

    private static string NormalizeCurrency(string currency)
    {
        var normalized = currency?.Trim().ToUpperInvariant() ?? string.Empty;

        if (normalized.Length != 3 || !normalized.All(char.IsAsciiLetterUpper))
            throw new DomainException("Currency must be a three-letter ISO code.");

        return normalized;
    }
}
