namespace PBMS_WPF_Application.Core.DTOs;

public class ParkingHistoryDto
{
    public int SessionId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string LicenseVehicle { get; set; } = string.Empty;
    public string FloorName { get; set; } = string.Empty;
    public string SlotName { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string SessionStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
