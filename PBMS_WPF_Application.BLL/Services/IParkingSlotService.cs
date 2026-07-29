using PBMS_WPF_Application.DAL.Entities;
using System.Collections.Generic;

namespace PBMS_WPF_Application.BLL.Services;

public interface IParkingSlotService
{
    List<ParkingSlot> GetSlotsByFloor(int floorId);
    bool CheckIn(int slotId, int userId, string licenseVehicle, out string message);
    bool CheckOut(int slotId, out string message);
    bool BookSlot(int slotId, int userId, string licenseVehicle, out string ticketCode, out string message);
    bool ConfirmCheckIn(string ticketCode, string licenseVehicle, out string message);
    bool ConfirmCheckOut(string ticketCode, string licenseVehicle, out string message);
    List<ParkingSession> GetActiveOrReservedSessions();
}
