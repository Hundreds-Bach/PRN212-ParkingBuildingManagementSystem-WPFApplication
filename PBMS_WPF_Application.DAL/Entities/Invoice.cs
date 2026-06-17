using System;
using System.Collections.Generic;

namespace PBMS_WPF_Application.DAL.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int SessionId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? PaymentTime { get; set; }

    public int StaffId { get; set; }

    public virtual ParkingSession Session { get; set; } = null!;

    public virtual User Staff { get; set; } = null!;
}
