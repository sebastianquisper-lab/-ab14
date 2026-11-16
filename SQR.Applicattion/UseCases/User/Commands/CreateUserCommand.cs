using MediatR;
using SQR.Domain.Ports;
using UserEntity = SQR.Domain.Entities.User;

namespace SQR.Applicattion.UseCases.User.Commands;

public sealed record CreateUserCommand(
    string Username,
    string PasswordHash,
    string? Email) : IRequest<UserEntity>;

internal sealed class CreateUserHandler(IUnitOfWork uow)
    : IRequestHandler<CreateUserCommand, UserEntity>
{
    public async Task<UserEntity> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var user = new UserEntity
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        return user;
    }
}
