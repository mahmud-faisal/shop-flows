using ShopFlows.Domain.Common;

namespace ShopFlows.Domain.Catalog;

public sealed class Category : Entity
{
    public Category(string name, string? description = null)
        : base(Guid.CreateVersion7())
    {
        Name = RequireName(name);
        Description = description?.Trim() ?? string.Empty;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public void Rename(string newName) => Name = RequireName(newName);

    public void Describe(string? description) => Description = description?.Trim() ?? string.Empty;

    private static string RequireName(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? throw new DomainException("Category name is required.")
            : name.Trim();
}
