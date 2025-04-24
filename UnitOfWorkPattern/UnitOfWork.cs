using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.UnitOfWorkPattern
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenericRepository<Book> Books { get; private set; }
        public IGenericRepository<Author> Authors { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }

        private AppDbContext db;

        public UnitOfWork(AppDbContext _db)
        {
            db = _db;
            Categories = new GenericRepository<Category>(db);
            Books = new GenericRepository<Book>(db);
            Authors = new GenericRepository<Author>(db);
           

        }

        public async Task<int> CommitChanges()
        {
            
            return await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
