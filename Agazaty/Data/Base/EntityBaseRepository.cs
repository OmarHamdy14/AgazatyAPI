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
        public T Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = false)
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
            return query.FirstOrDefault();

        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProp = null)
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
            return query.ToList();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
            _appDbContext.SaveChanges();
        }
        public void Update(T entity)
        {
            EntityEntry e = _appDbContext.Entry<T>(entity);
            e.State = EntityState.Modified;
            _appDbContext.SaveChanges();
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
            _appDbContext.SaveChanges();
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
            _appDbContext.SaveChanges();
        }
    }
}
