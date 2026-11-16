using MediatR;
using SQR.Domain.Ports;

namespace SQR.Applicattion.UseCases.Ticket.Commands;

public sealed record UpdateTicketStatusCommand(Guid Id, string Status) : IRequest<bool>;

internal sealed class UpdateTicketStatusHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTicketStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateTicketStatusCommand request, CancellationToken ct)
    {
        var ticket = await uow.Tickets.GetByIdAsync(request.Id, ct);
        if (ticket is null)
            return false;

        if (request.Status is not ("abierto" or "en progreso" or "cerrado"))
            throw new ArgumentException("Estado no válido.");

        ticket.Status = request.Status;
        ticket.ClosedAt = request.Status == "cerrado"
            ? DateTime.UtcNow
            : null;

        uow.Tickets.Update(ticket);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
