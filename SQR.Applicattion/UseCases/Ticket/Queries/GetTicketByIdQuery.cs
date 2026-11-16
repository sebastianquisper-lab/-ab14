using MediatR;
using SQR.Domain.Ports;
using TicketEntity = SQR.Domain.Entities.Ticket;

namespace SQR.Applicattion.UseCases.Ticket.Queries;

public sealed record GetTicketByIdQuery(Guid Id) : IRequest<TicketEntity?>;

internal sealed class GetTicketByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketByIdQuery, TicketEntity?>
{
    public Task<TicketEntity?> Handle(GetTicketByIdQuery request, CancellationToken ct)
    {
        return uow.Tickets.GetByIdAsync(request.Id, ct);
    }
}
