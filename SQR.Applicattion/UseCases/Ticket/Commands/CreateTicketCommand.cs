using MediatR;
using SQR.Domain.Ports;
using TicketEntity = SQR.Domain.Entities.Ticket;

namespace SQR.Applicattion.UseCases.Ticket.Commands;

public sealed record CreateTicketCommand(
    Guid UserId,
    string Title,
    string? Description) : IRequest<TicketEntity>;

internal sealed class CreateTicketHandler(IUnitOfWork uow)
    : IRequestHandler<CreateTicketCommand, TicketEntity>
{
    public async Task<TicketEntity> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        var ticket = new TicketEntity
        {
            TicketId = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description,
            Status = "abierto",
            CreatedAt = DateTime.UtcNow
        };

        await uow.Tickets.AddAsync(ticket, ct);
        await uow.SaveChangesAsync(ct);

        return ticket;
    }
}
