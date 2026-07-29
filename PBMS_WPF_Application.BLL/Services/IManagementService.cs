using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;

namespace PBMS_WPF_Application.BLL.Services;

public interface IManagementService
{
    ManagerDashboardDto GetDashboard();
    List<ParkingHistoryDto> GetParkingHistory();
    List<User> GetUsers();
    List<Role> GetRoles();
    List<Floor> GetFloors();
    List<ParkingSlot> GetSlots();
    List<VehiclesType> GetVehicleTypes();
    bool UpdateUserRole(int currentAdminId, int userId, int roleId, out string message);
    bool ToggleUser(int currentAdminId, int userId, bool isDeleted, out string message);
    bool SaveFloor(int? floorId, string floorName, int capacity, out string message);
    bool SaveSlot(int? slotId, string slotName, int floorId, int typeId, string status, out string message);
    bool ToggleSlot(int slotId, bool isDeleted, out string message);
    bool UpdateVehiclePrice(int typeId, decimal price, out string message);
}
