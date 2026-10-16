using ShopFlows.Domain.Common;

namespace ShopFlows.Domain.Catalog;

public sealed class Product : Entity
{
    public Product(string sku, string name, Money price, Guid categoryId)
        : base(Guid.CreateVersion7())
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU is required.");
        if (categoryId == Guid.Empty)
            throw new DomainException("Product must belong to a category.");

        Sku = sku.Trim().ToUpperInvariant();
        Name = RequireName(name);
        Price = RequirePositive(price);
        CategoryId = categoryId;
        IsActive = true;
    }

    public string Sku { get; }

    public string Name { get; private set; }

    public Money Price { get; private set; }

    public Guid CategoryId { get; private set; }

    public bool IsActive { get; private set; }

    public void Rename(string newName) => Name = RequireName(newName);

    public void ChangePrice(Money newPrice)
    {
        if (newPrice.Currency != Price.Currency)
            throw new DomainException("A price change cannot switch currency.");

        Price = RequirePositive(newPrice);
    }

    public void MoveToCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Product must belong to a category.");

        CategoryId = categoryId;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private static string RequireName(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? throw new DomainException("Product name is required.")
            : name.Trim();

    private static Money RequirePositive(Money price) =>
        price.Amount <= 0
            ? throw new DomainException("Product price must be greater than zero.")
            : price;
}
