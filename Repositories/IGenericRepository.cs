using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        //add methods without implementation
        //repo layer contains all the generic methods that are used to interact with the database

        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        //Task<T> GetAllWithAuthors();
        //Task<T> GetAllWithCategories();
        Task Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> FindByType(Expression<Func<T, bool>> predicate);
        Task<bool> IsUnique(Expression<Func<T, bool>> predicate);
        //a methode that takes a list of expressions and returns a list of entities
        //A params array that allows passing multiple lambda expressions to specify which navigation properties to include.
        //Expression<Func<T, object>> is the syntax used by Entity Framework to describe LINQ expressions (like .Include(b => b.Author)).
        Task<IEnumerable<T>> GetAllIncluding(params Expression<Func<T, object>>[] includes);

        //T--> ClassType,predicate-->Condition of filteration
        //This represents a lambda expression or a filter condition that takes an input of type T (the entity type) and returns a bool.
        //The bool here is the return type of the function inside the Expression.
    }
}
