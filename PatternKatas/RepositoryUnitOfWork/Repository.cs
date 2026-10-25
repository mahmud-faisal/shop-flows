namespace PatternKatas.RepositoryUnitOfWork;


public interface IRepository<T> where T : IEntity{
    T? GetById(Guid Id);
    IReadOnlyList<T> List();
    void Add(T entity);
    void Remove(T entity);
}
public sealed class Repository<T>(InMemoryDatabase database,UnitOfWork unitOfWork) : IRepository<T> where T:IEntity
{
public T? GetById(Guid id) => database.Rows<T>().FirstOrDefault(row => row.Id == id);

public IReadOnlyList<T> List() => database.Rows<T>();

public void Add(T entity) => unitOfWork.RegisterNew(entity);

public void Remove(T entity) => unitOfWork.RegisterRemoved(entity);
}
