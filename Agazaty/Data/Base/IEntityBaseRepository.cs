using System.Linq.Expressions;

namespace Agazaty.Data.Base
{
    public interface IEntityBaseRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProp = null);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
