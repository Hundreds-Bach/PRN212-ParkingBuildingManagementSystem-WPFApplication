using System;
using System.Collections.Generic;

namespace PBMS_WPF_Application.DAL.Entities;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public string TicketStatus { get; set; } = null!;

    public virtual ParkingSession? ParkingSession { get; set; }
}
