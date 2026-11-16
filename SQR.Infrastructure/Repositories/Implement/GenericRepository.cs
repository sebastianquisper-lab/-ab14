using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SQR.Domain.Ports;
using SQR.Persistence.Context;

namespace SQR.Infrastructure.Repositories.Implement;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly TicketSystemContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(TicketSystemContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(CancellationToken ct)
    {
        return await _dbSet.AsNoTracking().ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet.FindAsync([id], ct);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<List<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
