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

    public bool BookSlot(int slotId, int userId, string licenseVehicle, string ticketCode, out string message)
    {
        var slot = _context.ParkingSlots.FirstOrDefault(ps => ps.SlotId == slotId && !ps.IsDeleted);
        if (slot == null)
        {
            message = "Không tìm thấy ô đỗ xe.";
            return false;
        }

        if (!slot.SlotStatus.Equals("available", StringComparison.OrdinalIgnoreCase))
        {
            message = "Ô này không còn trống.";
            return false;
        }

        var ticket = new Ticket
        {
            TicketCode = ticketCode,
            TicketStatus = "Active"
        };

        var session = new ParkingSession
        {
            UserId = userId,
            SlotId = slot.SlotId,
            LicenseVehicle = licenseVehicle,
            TypeId = slot.TypeId,
            BookingTime = DateTime.Now,
            CheckInTime = null,
            CheckOutTime = null,
            SessionStatus = "Reserved",
            Ticket = ticket,
            IsDeleted = false
        };

        _context.Tickets.Add(ticket);
        _context.ParkingSessions.Add(session);
        slot.SlotStatus = "reserved";

        _context.SaveChanges();
        message = "Đặt chỗ thành công.";
        return true;
    }

    public bool ConfirmCheckIn(string ticketCode, string licenseVehicle, out string message)
    {
        var session = _context.ParkingSessions
            .Include(ps => ps.Ticket)
            .Include(ps => ps.Slot)
            .FirstOrDefault(ps => ps.Ticket.TicketCode == ticketCode 
                                  && ps.LicenseVehicle == licenseVehicle
                                  && ps.SessionStatus == "Reserved"
                                  && !ps.IsDeleted);

        if (session == null)
        {
            message = "Không tìm thấy thông tin đặt chỗ hợp lệ khớp với mã vé và biển số xe.";
            return false;
        }

        session.CheckInTime = DateTime.Now;
        session.SessionStatus = "InUse";
        session.Slot.SlotStatus = "occupied";

        _context.SaveChanges();
        message = $"Xác nhận Check-in thành công cho xe {licenseVehicle} vào ô {session.Slot.SlotName}.";
        return true;
    }

    public bool ConfirmCheckOut(string ticketCode, string licenseVehicle, out string message)
    {
        var session = _context.ParkingSessions
            .Include(ps => ps.Ticket)
            .Include(ps => ps.Slot)
            .FirstOrDefault(ps => ps.Ticket.TicketCode == ticketCode 
                                  && ps.LicenseVehicle == licenseVehicle
                                  && ps.SessionStatus == "InUse"
                                  && !ps.IsDeleted);

        if (session == null)
        {
            message = "Không tìm thấy lượt gửi xe đang hoạt động khớp với mã vé và biển số xe.";
            return false;
        }

        session.CheckOutTime = DateTime.Now;
        session.SessionStatus = "Completed";
        session.Ticket.TicketStatus = "Locked";
        session.Slot.SlotStatus = "available";

        _context.SaveChanges();
        message = $"Xác nhận Check-out thành công cho xe {licenseVehicle}. Ô đỗ {session.Slot.SlotName} đã được giải phóng.";
        return true;
    }

    public List<ParkingSession> GetActiveOrReservedSessions()
    {
        return _context.ParkingSessions
            .Include(ps => ps.Ticket)
            .Include(ps => ps.Slot)
            .Include(ps => ps.User)
            .Include(ps => ps.Type)
            .Where(ps => (ps.SessionStatus == "Reserved" || ps.SessionStatus == "InUse") && !ps.IsDeleted)
            .OrderByDescending(ps => ps.BookingTime)
            .ToList();
    }
}
