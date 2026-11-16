using SQR.Domain.Entities;
using SQR.Domain.Ports;
using SQR.Persistence.Context;

namespace SQR.Infrastructure.Repositories.Implement;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketSystemContext _context;
    private IGenericRepository<User>? _users;
    private IGenericRepository<Ticket>? _tickets;
    private IGenericRepository<Response>? _responses;
    private IGenericRepository<Role>? _roles;
    private IGenericRepository<UserRole>? _userRoles;

    public UnitOfWork(TicketSystemContext context)
    {
        _context = context;
    }

    public IGenericRepository<User> Users
        => _users ??= new GenericRepository<User>(_context);

    public IGenericRepository<Ticket> Tickets
        => _tickets ??= new GenericRepository<Ticket>(_context);

    public IGenericRepository<Response> Responses
        => _responses ??= new GenericRepository<Response>(_context);

    public IGenericRepository<Role> Roles
        => _roles ??= new GenericRepository<Role>(_context);

    public IGenericRepository<UserRole> UserRoles
        => _userRoles ??= new GenericRepository<UserRole>(_context);

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}
