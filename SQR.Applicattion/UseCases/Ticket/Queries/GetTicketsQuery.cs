using MediatR;
using SQR.Domain.Ports;
using TicketEntity = SQR.Domain.Entities.Ticket;

namespace SQR.Applicattion.UseCases.Ticket.Queries;

public sealed record GetTicketsQuery : IRequest<List<TicketEntity>>;

internal sealed class GetTicketsHandler(IUnitOfWork uow)
    : IRequestHandler<GetTicketsQuery, List<TicketEntity>>
{
    public Task<List<TicketEntity>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        return uow.Tickets.GetAllAsync(ct);
    }
}
