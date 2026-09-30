using System.Linq.Expressions;

namespace TitanFitness.Domain.Common;

public interface IRepository<T>
    where T : class, IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);


    Task<T?> FindAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default);

    void Add(T entity);

    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
