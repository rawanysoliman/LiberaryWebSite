using System;
using System.Linq.Expressions;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext db;
        private readonly DbSet<T> dbSet;

        public GenericRepository(AppDbContext _db) //constructor
        {
            db = _db;
            dbSet = db.Set<T>();
        }

        //implementation of methods

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        //return all entities that match the predicate
        public async Task<IEnumerable<T>> FindByType(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        //async Task<T> GetAllWithAuthors()
        //{
        //    return await dbSet.Include(b => b.).ToListAsync();

        //}
        //Task<T> GetAllWithCategories()
        //{

        //}

       //a method to include related entities
       public async Task<IEnumerable<T>> GetAllIncluding(params Expression<Func<T, object>>[] includes)
       {
        
          IQueryable<T> query = dbSet;
          if (includes != null)
          {
              foreach (var include in includes)
              {
                  if (include != null)
                  {
                      query = query.Include(include);
                  }
              }
          }
          return await query.ToListAsync();   
      }


       public async Task<bool> IsUnique(Expression<Func<T, bool>> predicate)
        {
             return !await dbSet.AnyAsync(predicate);
        }
    }
}
