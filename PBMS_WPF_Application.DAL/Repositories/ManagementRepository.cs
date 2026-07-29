using Microsoft.EntityFrameworkCore;
using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;

namespace PBMS_WPF_Application.DAL.Repositories;

public class ManagementRepository : IManagementRepository
{
    private readonly PbmsDbContext _context;

    public ManagementRepository() : this(new PbmsDbContext()) { }

    public ManagementRepository(PbmsDbContext context)
    {
        _context = context;
    }

    public ManagerDashboardDto GetDashboard()
    {
        return new ManagerDashboardDto
        {
            TotalSlots = _context.ParkingSlots.Count(s => !s.IsDeleted),
            AvailableSlots = _context.ParkingSlots.Count(s => !s.IsDeleted && s.SlotStatus == "available"),
            ReservedSlots = _context.ParkingSlots.Count(s => !s.IsDeleted && s.SlotStatus == "reserved"),
            OccupiedSlots = _context.ParkingSlots.Count(s => !s.IsDeleted && s.SlotStatus == "occupied"),
            TotalUsers = _context.Users.Count(u => !u.IsDeleted),
            ActiveSessions = _context.ParkingSessions.Count(s => !s.IsDeleted && s.SessionStatus == "InUse"),
            CompletedSessions = _context.ParkingSessions.Count(s => !s.IsDeleted && s.SessionStatus == "Completed"),
            TotalRevenue = _context.Invoices.Sum(i => (decimal?)i.TotalAmount) ?? 0
        };
    }

    public List<ParkingHistoryDto> GetParkingHistory()
    {
        return _context.ParkingSessions
            .AsNoTracking()
            .OrderByDescending(s => s.CheckInTime ?? s.BookingTime)
            .Select(s => new ParkingHistoryDto
            {
                SessionId = s.SessionId,
                Username = s.User.Username,
                LicenseVehicle = s.LicenseVehicle,
                FloorName = s.Slot.Floor.FloorName,
                SlotName = s.Slot.SlotName,
                CheckInTime = s.CheckInTime,
                CheckOutTime = s.CheckOutTime,
                SessionStatus = s.SessionStatus,
                TotalAmount = s.Invoices.Sum(i => (decimal?)i.TotalAmount) ?? 0
            }).ToList();
    }

    public List<User> GetUsers() => _context.Users.AsNoTracking().Include(u => u.Role).OrderBy(u => u.UserId).ToList();
    public List<Role> GetRoles() => _context.Roles.AsNoTracking().Where(r => !r.IsDeleted).OrderBy(r => r.RoleId).ToList();
    public List<Floor> GetFloors() => _context.Floors.AsNoTracking().OrderBy(f => f.FloorId).ToList();
    public List<ParkingSlot> GetSlots() => _context.ParkingSlots.AsNoTracking().Include(s => s.Floor).Include(s => s.Type).OrderBy(s => s.SlotId).ToList();
    public List<VehiclesType> GetVehicleTypes() => _context.VehiclesTypes.AsNoTracking().Where(t => !t.IsDeleted).OrderBy(t => t.TypeId).ToList();

    public bool UpdateUserRole(int userId, int roleId)
    {
        var user = _context.Users.Find(userId);
        if (user == null || !_context.Roles.Any(r => r.RoleId == roleId && !r.IsDeleted)) return false;
        user.RoleId = roleId;
        return _context.SaveChanges() > 0;
    }

    public bool SetUserDeleted(int userId, bool isDeleted)
    {
        var user = _context.Users.Find(userId);
        if (user == null) return false;
        user.IsDeleted = isDeleted;
        return _context.SaveChanges() > 0;
    }

    public bool AddFloor(string floorName, int capacity)
    {
        _context.Floors.Add(new Floor { FloorName = floorName, Capacity = capacity, IsDeleted = false });
        return _context.SaveChanges() > 0;
    }

    public bool UpdateFloor(int floorId, string floorName, int capacity)
    {
        var floor = _context.Floors.Find(floorId);
        if (floor == null) return false;
        floor.FloorName = floorName;
        floor.Capacity = capacity;
        return _context.SaveChanges() > 0;
    }

    public bool AddSlot(string slotName, int floorId, int typeId)
    {
        _context.ParkingSlots.Add(new ParkingSlot
        {
            SlotName = slotName, FloorId = floorId, TypeId = typeId,
            SlotStatus = "available", IsDeleted = false
        });
        return _context.SaveChanges() > 0;
    }

    public bool UpdateSlot(int slotId, string slotName, int floorId, int typeId, string status)
    {
        var slot = _context.ParkingSlots.Find(slotId);
        if (slot == null) return false;
        slot.SlotName = slotName;
        slot.FloorId = floorId;
        slot.TypeId = typeId;
        slot.SlotStatus = status;
        return _context.SaveChanges() > 0;
    }

    public bool SetSlotDeleted(int slotId, bool isDeleted)
    {
        var slot = _context.ParkingSlots.Find(slotId);
        if (slot == null) return false;
        slot.IsDeleted = isDeleted;
        return _context.SaveChanges() > 0;
    }

    public bool UpdateVehiclePrice(int typeId, decimal price)
    {
        var type = _context.VehiclesTypes.Find(typeId);
        if (type == null) return false;
        type.Price = price;
        return _context.SaveChanges() > 0;
    }
}
