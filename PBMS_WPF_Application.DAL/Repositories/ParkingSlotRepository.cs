using Microsoft.EntityFrameworkCore;
using PBMS_WPF_Application.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PBMS_WPF_Application.DAL.Repositories;

public class ParkingSlotRepository : IParkingSlotRepository
{
    private readonly PbmsDbContext _context;

    public ParkingSlotRepository()
    {
        _context = new PbmsDbContext();
    }

    public ParkingSlotRepository(PbmsDbContext context)
    {
        _context = context;
    }

    public List<ParkingSlot> GetSlotsByFloor(int floorId)
    {
        return _context.ParkingSlots
            .Include(ps => ps.Floor)
            .Include(ps => ps.Type)
            .Where(ps => ps.FloorId == floorId && !ps.IsDeleted)
            .ToList();
    }

    public bool CheckIn(int slotId, int userId, string licenseVehicle, out string message)
    {
        var slot = _context.ParkingSlots.FirstOrDefault(ps => ps.SlotId == slotId && !ps.IsDeleted);
        if (slot == null)
        {
            message = "Khong tim thay o do xe.";
            return false;
        }

        if (!slot.SlotStatus.Equals("available", StringComparison.OrdinalIgnoreCase))
        {
            message = "O nay khong con trong.";
            return false;
        }

        var ticket = new Ticket
        {
            TicketCode = "T" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            TicketStatus = "Active"
        };

        var session = new ParkingSession
        {
            UserId = userId,
            SlotId = slot.SlotId,
            LicenseVehicle = licenseVehicle,
            TypeId = slot.TypeId,
            BookingTime = DateTime.Now,
            CheckInTime = DateTime.Now,
            SessionStatus = "InUse",
            Ticket = ticket,
            IsDeleted = false
        };

        _context.Tickets.Add(ticket);
        _context.ParkingSessions.Add(session);
        slot.SlotStatus = "occupied";

        _context.SaveChanges();
        message = "Check-in thanh cong.";
        return true;
    }

    public bool CheckOut(int slotId, out string message)
    {
        var slot = _context.ParkingSlots.FirstOrDefault(ps => ps.SlotId == slotId && !ps.IsDeleted);
        if (slot == null)
        {
            message = "Khong tim thay o do xe.";
            return false;
        }

        var session = _context.ParkingSessions
            .Include(ps => ps.Ticket)
            .FirstOrDefault(ps => ps.SlotId == slotId
                                  && ps.CheckOutTime == null
                                  && !ps.IsDeleted
                                  && ps.SessionStatus == "InUse");

        if (session == null)
        {
            message = "Khong tim thay luot gui xe dang hoat dong cho o nay.";
            return false;
        }

        session.CheckOutTime = DateTime.Now;
        session.SessionStatus = "Completed";
        session.Ticket.TicketStatus = "Locked";
        slot.SlotStatus = "available";

        _context.SaveChanges();
        message = "Check-out thanh cong.";
        return true;
    }
}
