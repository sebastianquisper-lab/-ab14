using MediatR;
using SQR.Domain.Ports;
using ResponseEntity = SQR.Domain.Entities.Response;

namespace SQR.Applicattion.UseCases.Response.Queries;

public sealed record GetResponsesByTicketQuery(Guid TicketId) : IRequest<List<ResponseEntity>>;

internal sealed class GetResponsesByTicketHandler(IUnitOfWork uow)
    : IRequestHandler<GetResponsesByTicketQuery, List<ResponseEntity>>
{
    public Task<List<ResponseEntity>> Handle(
        GetResponsesByTicketQuery request,
        CancellationToken ct)
    {
        return uow.Responses.FindAsync(x => x.TicketId == request.TicketId, ct);
    }
}
