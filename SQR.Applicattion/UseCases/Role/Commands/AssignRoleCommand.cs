using MediatR;
using SQR.Domain.Entities;
using SQR.Domain.Ports;

namespace SQR.Applicattion.UseCases.Role.Commands;

public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<bool>;

internal sealed class AssignRoleHandler(IUnitOfWork uow)
    : IRequestHandler<AssignRoleCommand, bool>
{
    public async Task<bool> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(request.UserId, ct);
        var role = await uow.Roles.GetByIdAsync(request.RoleId, ct);

        if (user is null || role is null)
            return false;

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = request.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await uow.UserRoles.AddAsync(userRole, ct);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
