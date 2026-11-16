using MediatR;
using SQR.Domain.Ports;
using UserEntity = SQR.Domain.Entities.User;

namespace SQR.Applicattion.UseCases.User.Queries;

public sealed record LoginQuery(string Username, string Password) : IRequest<UserEntity?>;

internal sealed class LoginHandler(IUnitOfWork uow)
    : IRequestHandler<LoginQuery, UserEntity?>
{
    public async Task<UserEntity?> Handle(LoginQuery request, CancellationToken ct)
    {
        var user = await uow.Users.FirstOrDefaultAsync(
            x => x.Username == request.Username,
            ct);

        if (user is null || user.PasswordHash != request.Password)
            return null;

        return user;
    }
}
