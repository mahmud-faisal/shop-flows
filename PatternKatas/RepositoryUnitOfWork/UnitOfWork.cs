namespace PatternKatas.RepositoryUnitOfWork;

public sealed class UnitOfWork
{
    private readonly InMemoryDatabase _database;
    private readonly List<IEntity> _new = [];
    private readonly List<IEntity> _removed = [];

    public UnitOfWork(InMemoryDatabase database)
    {
        _database = database;
    }

    public int PendingChanges => _new.Count + _removed.Count;
    
    internal void RegisterNew(IEntity entity) => _new.Add(entity);
    internal void RegisterRemoved(IEntity entity) => _removed.Add(entity);

    public void Commit(){
        var snapshot = _database.Snapshot();

        try{
            _new.ForEach(_database.Insert);
            _removed.ForEach(_database.Delete);

        }
        catch{
            _database.Restore(snapshot);
            throw;
        }
        finally{
            Discard();
        }
    }

    public void Discard(){
        _new.Clear();
        _removed.Clear();
    }

}
