using PBMS_WPF_Application.DAL.Entities;
using PBMS_WPF_Application.DAL.Repositories;
using System.Collections.Generic;

namespace PBMS_WPF_Application.BLL.Services;

public class ParkingSlotService : IParkingSlotService
{
    private readonly IParkingSlotRepository _parkingSlotRepository;

    public ParkingSlotService()
    {
        _parkingSlotRepository = new ParkingSlotRepository();
    }

    public ParkingSlotService(IParkingSlotRepository parkingSlotRepository)
    {
        _parkingSlotRepository = parkingSlotRepository;
    }

    public List<ParkingSlot> GetSlotsByFloor(int floorId)
    {
        return _parkingSlotRepository.GetSlotsByFloor(floorId);
    }
}
