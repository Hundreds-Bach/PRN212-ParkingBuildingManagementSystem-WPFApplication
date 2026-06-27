using PBMS_WPF_Application.DAL.Entities;
using System.Collections.Generic;

namespace PBMS_WPF_Application.DAL.Repositories;

public interface IParkingSlotRepository
{
    List<ParkingSlot> GetSlotsByFloor(int floorId);
    bool CheckIn(int slotId, int userId, string licenseVehicle, out string message);
    bool CheckOut(int slotId, out string message);
}
