using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQR.Applicattion.UseCases.Response.Commands;
using SQR.Applicattion.UseCases.Response.Queries;

namespace SQR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResponsesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ticket/{ticketId:guid}")]
    public async Task<IActionResult> GetByTicket(Guid ticketId, CancellationToken ct)
    {
        var responses = await _mediator.Send(
            new GetResponsesByTicketQuery(ticketId),
            ct);

        return Ok(responses);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateResponseCommand command,
        CancellationToken ct)
    {
        var response = await _mediator.Send(command, ct);
        return Ok(response);
    }
}
