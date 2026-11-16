using SQR.Domain.Entities;

namespace SQR.Domain.Ports;

public interface IExcelReportGenerator
{
    byte[] GenerarReporteUsuarios(IEnumerable<User> usuarios);
}