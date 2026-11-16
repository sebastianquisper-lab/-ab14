using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQR.Applicattion.UseCases.Ticket.Commands;
using SQR.Applicattion.UseCases.Ticket.Queries;

namespace SQR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        // 1. Obtenemos los tickets usando el handler existente de MediatR
        var tickets = await _mediator.Send(new GetTicketsQuery(), ct);

        // 2. Construimos el Excel con ClosedXML
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Reporte Tickets");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Ticket ID";
            worksheet.Cell(1, 2).Value = "Título";
            worksheet.Cell(1, 3).Value = "Descripción";
            worksheet.Cell(1, 4).Value = "Estado";

            // Estilos del Encabezado
            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Turquoise;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Relleno de datos
            int row = 2;
            foreach (var t in tickets)
            {
                worksheet.Cell(row, 1).Value = t.TicketId.ToString();
                worksheet.Cell(row, 2).Value = t.Title;
                worksheet.Cell(row, 3).Value = t.Description;
                worksheet.Cell(row, 4).Value = t.Status;
                row++;
            }

            var tableRange = worksheet.Range(1, 1, Math.Max(2, row - 1), 4);
            tableRange.CreateTable();
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "ReporteTickets.xlsx"
                );
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var tickets = await _mediator.Send(new GetTicketsQuery(), ct);
        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var ticket = await _mediator.Send(new GetTicketByIdQuery(id), ct);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTicketCommand command,
        CancellationToken ct)
    {
        var ticket = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = ticket.TicketId }, ticket);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken ct)
    {
        var ok = await _mediator.Send(
            new UpdateTicketStatusCommand(id, request.Status),
            ct);

        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteTicketCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    public sealed record UpdateStatusRequest(string Status);
}