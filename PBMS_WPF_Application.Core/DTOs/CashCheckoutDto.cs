namespace PBMS_WPF_Application.Core.DTOs;

public class CashCheckoutDto
{
    public int SessionId { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string LicenseVehicle { get; set; } = string.Empty;
    public string SlotName { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public int ChargedHours { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal TotalAmount { get; set; }
}
