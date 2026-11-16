using MediatR;
using SQR.Domain.Ports;
using UserEntity = SQR.Domain.Entities.User;

namespace SQR.Applicattion.UseCases.User.Queries;

public sealed record GetUsersQuery : IRequest<List<UserEntity>>;

internal sealed class GetUsersHandler(IUnitOfWork uow)
    : IRequestHandler<GetUsersQuery, List<UserEntity>>
{
    public Task<List<UserEntity>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        return uow.Users.GetAllAsync(ct);
    }
}
