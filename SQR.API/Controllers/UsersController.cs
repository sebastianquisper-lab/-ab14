using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQR.Applicattion.UseCases.User.Commands;
using SQR.Applicattion.UseCases.User.Queries;

namespace SQR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _mediator.Send(new GetUsersQuery(), ct);
        return Ok(users);
    }

    // --- NUEVO ENDPOINT DE EXPORTACIÓN ---
    [HttpGet("export/excel")]
    [Authorize]
    public async Task<IActionResult> ExportToExcel(CancellationToken ct)
    {
        var fileBytes = await _mediator.Send(new ExportUsersQuery(), ct);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ReporteUsuarios.xlsx"
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command,
        CancellationToken ct)
    {
        var user = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteUserCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }
}