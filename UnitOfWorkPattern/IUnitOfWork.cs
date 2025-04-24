using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.UnitOfWorkPattern
{
    public interface IUnitOfWork : IDisposable
    // Define the repositories as properties
    // This allows you to access them through the unit of work
    // without needing to create instances of them each time.
    // The repositories are generic, so they can work with any entity type.
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Book> Books { get; }
        IGenericRepository<Author> Authors { get; }

        Task<int> CommitChanges();

    }
}
