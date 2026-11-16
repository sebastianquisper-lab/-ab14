using MediatR;
using SQR.Domain.Ports;
using UserEntity = SQR.Domain.Entities.User;

namespace SQR.Applicattion.UseCases.User.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserEntity?>;

internal sealed class GetUserByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetUserByIdQuery, UserEntity?>
{
    public Task<UserEntity?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        return uow.Users.GetByIdAsync(request.Id, ct);
    }
}
