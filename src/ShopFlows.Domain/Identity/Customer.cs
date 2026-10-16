using ShopFlows.Domain.Common;

namespace ShopFlows.Domain.Identity;

public sealed class Customer : Entity
{
    public Customer(string name, string email)
        : base(Guid.CreateVersion7())
    {
        Name = RequireName(name);
        Email = RequireEmail(email);
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public void Rename(string newName) => Name = RequireName(newName);

    public void ChangeEmail(string newEmail) => Email = RequireEmail(newEmail);

    private static string RequireName(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? throw new DomainException("Customer name is required.")
            : name.Trim();

    private static string RequireEmail(string email)
    {
        var trimmed = email?.Trim().ToLowerInvariant() ?? string.Empty;
        var at = trimmed.IndexOf('@');
        var domain = at > 0 ? trimmed[(at + 1)..] : string.Empty;

        var isValid = at > 0
            && trimmed.Count(c => c == '@') == 1
            && !trimmed.Contains(' ')
            && domain.Contains('.')
            && !domain.StartsWith('.')
            && !domain.EndsWith('.');

        return isValid ? trimmed : throw new DomainException($"'{email}' is not a valid email address.");
    }
}
