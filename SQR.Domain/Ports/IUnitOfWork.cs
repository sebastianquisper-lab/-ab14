using SQR.Domain.Entities;

namespace SQR.Domain.Ports;

public interface IUnitOfWork
{
    IGenericRepository<User> Users { get; }

    IGenericRepository<Ticket> Tickets { get; }

    IGenericRepository<Response> Responses { get; }

    IGenericRepository<Role> Roles { get; }

    IGenericRepository<UserRole> UserRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
