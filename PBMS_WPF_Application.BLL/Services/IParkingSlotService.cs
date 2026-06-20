using PBMS_WPF_Application.DAL.Entities;
using System.Collections.Generic;

namespace PBMS_WPF_Application.BLL.Services;

public interface IParkingSlotService
{
    List<ParkingSlot> GetSlotsByFloor(int floorId);
}
