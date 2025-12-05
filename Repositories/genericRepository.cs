using Microsoft.EntityFrameworkCore;
using ScholaAi.Repositories.Base;
using ScholaAi.Models;
using System;

namespace ScholaAi.Repositories
{
    public class genericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DBcontext _context;
        protected readonly DbSet<T> _dbSet;

        public genericRepository(DBcontext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task addAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task deleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> getAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> getByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task updateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
