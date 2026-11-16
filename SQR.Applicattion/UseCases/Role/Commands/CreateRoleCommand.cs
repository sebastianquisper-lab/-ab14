using MediatR;
using SQR.Domain.Ports;
using RoleEntity = SQR.Domain.Entities.Role;

namespace SQR.Applicattion.UseCases.Role.Commands;

public sealed record CreateRoleCommand(string RoleName) : IRequest<RoleEntity>;

internal sealed class CreateRoleHandler(IUnitOfWork uow)
    : IRequestHandler<CreateRoleCommand, RoleEntity>
{
    public async Task<RoleEntity> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var role = new RoleEntity
        {
            RoleId = Guid.NewGuid(),
            RoleName = request.RoleName
        };

        await uow.Roles.AddAsync(role, ct);
        await uow.SaveChangesAsync(ct);

        return role;
    }
}
