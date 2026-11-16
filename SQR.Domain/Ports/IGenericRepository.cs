using System.Linq.Expressions;

namespace SQR.Domain.Ports;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(CancellationToken ct);

    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);

    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);

    Task AddAsync(T entity, CancellationToken ct);

    void Update(T entity);

    void Delete(T entity);
}
