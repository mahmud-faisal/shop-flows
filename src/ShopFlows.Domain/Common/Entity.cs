namespace ShopFlows.Domain.Common;

/// <summary>
/// Base type for objects that have an identity. Two entities are equal when they
/// are the same type and share the same Id, regardless of their other state.
/// </summary>
public abstract class Entity
{
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Entity id cannot be empty.");

        Id = id;
    }

    public Guid Id { get; }

    public override bool Equals(object? obj) =>
        obj is Entity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
