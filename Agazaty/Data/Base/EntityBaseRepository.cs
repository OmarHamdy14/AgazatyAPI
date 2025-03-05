using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Agazaty.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Agazaty.Data.Base;

namespace Agazaty.Data.Base
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        internal DbSet<T> dbSet;
        public EntityBaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            this.dbSet = _appDbContext.Set<T>();
        }
        public async Task<T> Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = false)
        {

            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProp))
            {
                foreach (var prop in includeProp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProp = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProp))
            {
                foreach (var prop in includeProp.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return await query.ToListAsync();
        }
        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task Update(T entity)
        {
            EntityEntry e = _appDbContext.Entry<T>(entity);
            e.State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
        }
        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
