using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQR.Applicattion.UseCases.Role.Commands;
using SQR.Applicattion.UseCases.Role.Queries;

namespace SQR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var roles = await _mediator.Send(new GetRolesQuery(), ct);
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleCommand command,
        CancellationToken ct)
    {
        var role = await _mediator.Send(command, ct);
        return Ok(role);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign(
        [FromBody] AssignRoleCommand command,
        CancellationToken ct)
    {
        var ok = await _mediator.Send(command, ct);
        return ok ? Ok() : NotFound();
    }
}
