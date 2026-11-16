using MediatR;
using Microsoft.AspNetCore.Mvc;
using SQR.Applicattion.UseCases.User.Queries;
using SQR.Domain.Ports;

namespace SQR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwt;
    private readonly IUnitOfWork _uow;

    public AuthController(IMediator mediator, IJwtTokenService jwt, IUnitOfWork uow)
    {
        _mediator = mediator;
        _jwt = jwt;
        _uow = uow;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var user = await _mediator.Send(
            new LoginQuery(request.Username, request.Password),
            ct);

        if (user is null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        var userRoles = await _uow.UserRoles.FindAsync(x => x.UserId == user.UserId, ct);
        var roleIds = userRoles.Select(x => x.RoleId).ToHashSet();
        var roles = (await _uow.Roles.GetAllAsync(ct))
            .Where(r => roleIds.Contains(r.RoleId))
            .Select(r => r.RoleName)
            .ToList();

        var token = _jwt.GenerateToken(user, roles);

        return Ok(new
        {
            token,
            userId = user.UserId,
            username = user.Username,
            roles
        });
    }

    public sealed record LoginRequest(string Username, string Password);
}
