using MediatR;
using SQR.Domain.Ports;

namespace SQR.Applicattion.UseCases.Ticket.Commands;

public sealed record DeleteTicketCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteTicketHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteTicketCommand, bool>
{
    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken ct)
    {
        var ticket = await uow.Tickets.GetByIdAsync(request.Id, ct);
        if (ticket is null)
            return false;

        uow.Tickets.Delete(ticket);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
