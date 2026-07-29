using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;

namespace PBMS_WPF_Application.DAL.Repositories;

public interface IManagementRepository
{
    ManagerDashboardDto GetDashboard();
    List<ParkingHistoryDto> GetParkingHistory();
    List<User> GetUsers();
    List<Role> GetRoles();
    List<Floor> GetFloors();
    List<ParkingSlot> GetSlots();
    List<VehiclesType> GetVehicleTypes();
    bool UpdateUserRole(int userId, int roleId);
    bool SetUserDeleted(int userId, bool isDeleted);
    bool AddFloor(string floorName, int capacity);
    bool UpdateFloor(int floorId, string floorName, int capacity);
    bool AddSlot(string slotName, int floorId, int typeId);
    bool UpdateSlot(int slotId, string slotName, int floorId, int typeId, string status);
    bool SetSlotDeleted(int slotId, bool isDeleted);
    bool UpdateVehiclePrice(int typeId, decimal price);
}
