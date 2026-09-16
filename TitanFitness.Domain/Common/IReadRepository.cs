using System.Linq.Expressions;

namespace TitanFitness.Domain.Common;
public interface IReadRepository<T>
    where T : IEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> FindAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default);

    IQueryable<T> Query();
}

