using MediatR;
using SQR.Domain.Ports;
using RoleEntity = SQR.Domain.Entities.Role;

namespace SQR.Applicattion.UseCases.Role.Queries;

public sealed record GetRolesQuery : IRequest<List<RoleEntity>>;

internal sealed class GetRolesHandler(IUnitOfWork uow)
    : IRequestHandler<GetRolesQuery, List<RoleEntity>>
{
    public Task<List<RoleEntity>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        return uow.Roles.GetAllAsync(ct);
    }
}
