using System;
using System.Collections.Generic;

namespace PBMS_WPF_Application.DAL.Entities;

public partial class VehiclesType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
}
