using System;
using System.Collections.Generic;

namespace SQR.Domain.Entities;

public partial class Response
{
    public Guid ResponseId { get; set; }

    public string Message { get; set; } = null!;

    public Guid TicketId { get; set; }

    public Guid ResponderId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Responder { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
