using MediatR;
using SQR.Domain.Ports;

namespace SQR.Applicattion.UseCases.User.Commands;

public sealed record DeleteUserCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteUserHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(request.Id, ct);
        if (user is null)
            return false;

        uow.Users.Delete(user);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
