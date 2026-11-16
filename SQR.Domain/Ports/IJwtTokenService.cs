using SQR.Domain.Entities;

namespace SQR.Domain.Ports;

public interface IJwtTokenService
{
    string GenerateToken(User user, IEnumerable<string> roles);
}
