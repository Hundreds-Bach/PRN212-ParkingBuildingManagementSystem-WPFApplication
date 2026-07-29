using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;
using PBMS_WPF_Application.DAL.Repositories;

namespace PBMS_WPF_Application.BLL.Services;

public class ManagementService : IManagementService
{
    private readonly IManagementRepository _repository;

    public ManagementService() : this(new ManagementRepository()) { }
    public ManagementService(IManagementRepository repository) => _repository = repository;

    public ManagerDashboardDto GetDashboard() => _repository.GetDashboard();
    public List<ParkingHistoryDto> GetParkingHistory() => _repository.GetParkingHistory();
    public List<User> GetUsers() => _repository.GetUsers();
    public List<Role> GetRoles() => _repository.GetRoles();
    public List<Floor> GetFloors() => _repository.GetFloors();
    public List<ParkingSlot> GetSlots() => _repository.GetSlots();
    public List<VehiclesType> GetVehicleTypes() => _repository.GetVehicleTypes();

    public bool UpdateUserRole(int currentAdminId, int userId, int roleId, out string message)
    {
        if (currentAdminId == userId)
        {
            message = "Admin không thể tự thay đổi quyền của chính mình.";
            return false;
        }
        bool result = _repository.UpdateUserRole(userId, roleId);
        message = result ? "Đã cập nhật quyền người dùng." : "Không thể cập nhật quyền.";
        return result;
    }

    public bool ToggleUser(int currentAdminId, int userId, bool isDeleted, out string message)
    {
        if (currentAdminId == userId)
        {
            message = "Admin không thể khóa tài khoản đang đăng nhập.";
            return false;
        }
        bool result = _repository.SetUserDeleted(userId, isDeleted);
        message = result ? (isDeleted ? "Đã khóa tài khoản." : "Đã mở khóa tài khoản.") : "Không thể cập nhật tài khoản.";
        return result;
    }

    public bool SaveFloor(int? floorId, string floorName, int capacity, out string message)
    {
        if (string.IsNullOrWhiteSpace(floorName) || capacity <= 0)
        {
            message = "Tên tầng và sức chứa phải hợp lệ.";
            return false;
        }
        bool result = floorId.HasValue
            ? _repository.UpdateFloor(floorId.Value, floorName.Trim(), capacity)
            : _repository.AddFloor(floorName.Trim(), capacity);
        message = result ? "Đã lưu thông tin tầng." : "Không thể lưu tầng.";
        return result;
    }

    public bool SaveSlot(int? slotId, string slotName, int floorId, int typeId, string status, out string message)
    {
        string[] validStatuses = ["available", "reserved", "occupied"];
        status = status.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slotName) || !validStatuses.Contains(status))
        {
            message = "Tên slot hoặc trạng thái không hợp lệ.";
            return false;
        }
        bool result = slotId.HasValue
            ? _repository.UpdateSlot(slotId.Value, slotName.Trim(), floorId, typeId, status)
            : _repository.AddSlot(slotName.Trim(), floorId, typeId);
        message = result ? "Đã lưu slot." : "Không thể lưu slot.";
        return result;
    }

    public bool ToggleSlot(int slotId, bool isDeleted, out string message)
    {
        bool result = _repository.SetSlotDeleted(slotId, isDeleted);
        message = result ? (isDeleted ? "Đã vô hiệu hóa slot." : "Đã khôi phục slot.") : "Không thể cập nhật slot.";
        return result;
    }

    public bool UpdateVehiclePrice(int typeId, decimal price, out string message)
    {
        if (price < 0)
        {
            message = "Giá không được âm.";
            return false;
        }
        bool result = _repository.UpdateVehiclePrice(typeId, price);
        message = result ? "Đã cập nhật giá." : "Không thể cập nhật giá.";
        return result;
    }
}
