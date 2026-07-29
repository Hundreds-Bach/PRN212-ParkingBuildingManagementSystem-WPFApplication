namespace PBMS_WPF_Application.Core.DTOs;

public class ManagerDashboardDto
{
    public int TotalSlots { get; set; }
    public int AvailableSlots { get; set; }
    public int ReservedSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveSessions { get; set; }
    public int CompletedSessions { get; set; }
    public decimal TotalRevenue { get; set; }
}
