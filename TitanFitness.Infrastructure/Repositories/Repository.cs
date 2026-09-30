using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TitanFitness.Domain.Common;
using TitanFitness.Infrastructure.Data;

namespace TitanFitness.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class, IAggregateRoot
{
    private readonly ApplicationDbContext _context ;

    public  Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
    return  _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    }

    public Task<T?> FindAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
      return  _context.Set<T>()
           .FirstOrDefaultAsync(predicate, cancellationToken);

    }


    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
}


