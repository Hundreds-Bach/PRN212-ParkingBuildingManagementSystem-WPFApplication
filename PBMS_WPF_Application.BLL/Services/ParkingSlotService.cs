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

    public bool CheckIn(int slotId, int userId, string licenseVehicle, out string message)
    {
        if (slotId <= 0)
        {
            message = "Vui long chon o do xe.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(licenseVehicle))
        {
            message = "Vui long nhap bien so xe.";
            return false;
        }

        return _parkingSlotRepository.CheckIn(slotId, userId, licenseVehicle.Trim(), out message);
    }

    public bool CheckOut(int slotId, out string message)
    {
        if (slotId <= 0)
        {
            message = "Vui long chon o do xe.";
            return false;
        }

        return _parkingSlotRepository.CheckOut(slotId, out message);
    }
}
