using MediatR;
using SQR.Domain.Ports;
using ResponseEntity = SQR.Domain.Entities.Response;

namespace SQR.Applicattion.UseCases.Response.Commands;

public sealed record CreateResponseCommand(
    Guid TicketId,
    Guid ResponderId,
    string Message) : IRequest<ResponseEntity>;

internal sealed class CreateResponseHandler(IUnitOfWork uow)
    : IRequestHandler<CreateResponseCommand, ResponseEntity>
{
    public async Task<ResponseEntity> Handle(
        CreateResponseCommand request,
        CancellationToken ct)
    {
        var response = new ResponseEntity
        {
            ResponseId = Guid.NewGuid(),
            TicketId = request.TicketId,
            ResponderId = request.ResponderId,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Responses.AddAsync(response, ct);
        await uow.SaveChangesAsync(ct);

        return response;
    }
}
