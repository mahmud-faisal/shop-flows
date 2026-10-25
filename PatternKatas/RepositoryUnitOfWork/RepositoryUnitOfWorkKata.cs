namespace PatternKatas.RepositoryUnitOfWork;

public sealed record Book(Guid Id, string Title) : IEntity;
public sealed record Member(Guid Id,string Name) : IEntity;

public sealed class RepositoryUnitOfWorkKata
{
    public static void Run(){
        var database = new InMemoryDatabase();
        var unitOfWork = new UnitOfWork(database);
        IRepository<Book> books = new Repository<Book>(database, unitOfWork);
        IRepository<Member> members = new Repository<Member>(database, unitOfWork);
        
         Console.WriteLine("1) Two repositories, one unit of work, one commit");

         var dune = new Book(Guid.NewGuid(), "Dune");
        books.Add(dune);
        members.Add(new Member(Guid.NewGuid(), "Ada"));

          Console.WriteLine($"   before commit: {books.List().Count} books, {members.List().Count} members, {unitOfWork.PendingChanges} pending");
       
        unitOfWork.Commit();
       
        Console.WriteLine($"   after commit:  {books.List().Count} books, {members.List().Count} members");

         Console.WriteLine("2) Changes that are discarded never reach the database");
        books.Add(new Book(Guid.NewGuid(), "Draft that was abandoned"));
        unitOfWork.Discard();
        Console.WriteLine($"   books: {books.List().Count}");

        Console.WriteLine("3) A failing commit is all-or-nothing");
        books.Add(new Book(Guid.NewGuid(), "Neuromancer"));
        books.Add(dune with { Title = "Dune (duplicate id)" });

        try
        {
            unitOfWork.Commit();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   commit failed: {ex.Message}");
        }
        Console.WriteLine($"   books: {string.Join(", ", books.List().Select(b => b.Title))} (Neuromancer was rolled back)");
    }
}
