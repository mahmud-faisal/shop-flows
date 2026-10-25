namespace PatternKatas.RepositoryUnitOfWork;

public interface IEntity{
    Guid Id{get;}
}


public sealed class InMemoryDatabase
{
private readonly Dictionary<Type, List<IEntity>> _tables = new();

public IReadOnlyList<T> Rows<T>() where T : IEntity => Table(typeof(T)).Cast<T>().ToList();

internal void Insert(IEntity entity){
        var table = Table(entity.GetType());
        if (table.Any(row => row.Id == entity.Id))
            throw new InvalidOperationException($"Duplicate id {entity.Id} in {entity.GetType().Name}");
        table.Add(entity);
}

internal void Delete(IEntity entity) => Table(entity.GetType()).RemoveAll(row => row.Id == entity.Id);

internal Dictionary<Type, List<IEntity>> Snapshot() => _tables.ToDictionary(pair => pair.Key, pair => pair.Value.ToList());

internal void Restore(Dictionary<Type, List<IEntity>> snapshot){
        _tables.Clear();
        foreach (var (type, rows) in snapshot){
            _tables[type] = rows;
        }
}

private List<IEntity> Table(Type type){
        if (!_tables.TryGetValue(type, out var table))
            _tables[type] = table = new List<IEntity>();
            return table;
}


}
