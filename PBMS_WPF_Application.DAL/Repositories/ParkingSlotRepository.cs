using Microsoft.EntityFrameworkCore;
using PBMS_WPF_Application.DAL.Entities;
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
}
