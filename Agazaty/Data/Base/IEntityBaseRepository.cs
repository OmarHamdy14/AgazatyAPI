using System.Linq.Expressions;

namespace Agazaty.Data.Base
{
    public interface IEntityBaseRepository<T> where T : class
    {
        Task<T> Get(Expression<Func<T, bool>> filter, string? includeProp = null, bool tracked = false);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProp = null);
        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entity);
    }
}
