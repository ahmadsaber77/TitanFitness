using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TitanFitness.Domain.Common;
using TitanFitness.Infrastructure.Data;

namespace TitanFitness.Infrastructure.Repositories;

public class ReadRepository<T> : IReadRepository<T> where T : class, IEntity
{
    private readonly ApplicationDbContext _context;

    public ReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task<T?> GetByIdAsync(
     Guid id,
     CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == id,
                cancellationToken);
    }

    public Task<T?> FindAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                predicate,
                cancellationToken);
    }

    public IQueryable<T> Query()
    {
        return _context.Set<T>()
            .AsNoTracking();
    }


}
