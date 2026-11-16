using MediatR;
using SQR.Domain.Entities;
using SQR.Domain.Ports;

namespace SQR.Applicattion.UseCases.User.Queries;

public record ExportUsersQuery() : IRequest<byte[]>;

public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, byte[]>
{
    private readonly IExcelReportGenerator _excelReportGenerator;
    private readonly IGenericRepository<Domain.Entities.User> _userRepository;

    public ExportUsersQueryHandler(
        IExcelReportGenerator excelReportGenerator,
        IGenericRepository<Domain.Entities.User> userRepository)
    {
        _excelReportGenerator = excelReportGenerator;
        _userRepository = userRepository;
    }

    public async Task<byte[]> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return _excelReportGenerator.GenerarReporteUsuarios(users);
    }
} 