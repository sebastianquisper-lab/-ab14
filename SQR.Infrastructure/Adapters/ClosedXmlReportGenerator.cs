using System.IO;
using System.Collections.Generic;
using ClosedXML.Excel;
using SQR.Domain.Entities;
using SQR.Domain.Ports;

namespace SQR.Infrastructure.Adapters;

public class ClosedXmlReportGenerator : IExcelReportGenerator
{
    public byte[] GenerarReporteUsuarios(IEnumerable<User> usuarios)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Usuarios");

            // Estilo para los encabezados 
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.Cyan;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Encabezados
            worksheet.Cell(1, 1).Value = "User ID";
            worksheet.Cell(1, 2).Value = "Username";
            worksheet.Cell(1, 3).Value = "Email";

            int row = 2;
            foreach (var u in usuarios)
            {
                // Propiedades exactas de tu entidad User
                worksheet.Cell(row, 1).Value = u.UserId.ToString();
                worksheet.Cell(row, 2).Value = u.Username;
                worksheet.Cell(row, 3).Value = u.Email;
                row++;
            }

            // Formatear como tabla 
            if (row > 2)
            {
                var range = worksheet.Range($"A1:C{row - 1}");
                range.CreateTable();
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}