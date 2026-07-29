using PBMS_WPF_Application.DAL.Entities;
using PBMS_WPF_Application.DAL.Repositories;
using PBMS_WPF_Application.Core.DTOs;
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

    public bool BookSlot(int slotId, int userId, string licenseVehicle, out string ticketCode, out string message)
    {
        ticketCode = string.Empty;
        if (slotId <= 0)
        {
            message = "Vui lòng chọn ô đỗ xe.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(licenseVehicle))
        {
            message = "Vui lòng nhập biển số xe.";
            return false;
        }

        try
        {
            var rand = new System.Random();
            bool isUnique = false;
            string code = string.Empty;
            var activeSessions = _parkingSlotRepository.GetActiveOrReservedSessions();
            int attempts = 0;
            while (!isUnique && attempts < 100)
            {
                code = "T" + rand.Next(100000, 999999);
                if (!activeSessions.Any(s => s.Ticket.TicketCode.Equals(code, System.StringComparison.OrdinalIgnoreCase)))
                {
                    isUnique = true;
                }
                attempts++;
            }

            if (!isUnique)
            {
                message = "Không thể tạo mã vé. Vui lòng thử lại.";
                return false;
            }

            ticketCode = code;
            return _parkingSlotRepository.BookSlot(slotId, userId, licenseVehicle.Trim(), ticketCode, out message);
        }
        catch (System.Exception ex)
        {
            ticketCode = string.Empty;
            message = $"Không thể đặt chỗ: {ex.GetBaseException().Message}";
            return false;
        }
    }

    public bool ConfirmCheckIn(string ticketCode, string licenseVehicle, out string message)
    {
        if (string.IsNullOrWhiteSpace(ticketCode))
        {
            message = "Vui lòng nhập mã vé.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(licenseVehicle))
        {
            message = "Vui lòng nhập biển số xe.";
            return false;
        }

        return _parkingSlotRepository.ConfirmCheckIn(ticketCode.Trim(), licenseVehicle.Trim(), out message);
    }

    public bool ConfirmCheckOut(string ticketCode, string licenseVehicle, out string message)
    {
        if (string.IsNullOrWhiteSpace(ticketCode))
        {
            message = "Vui lòng nhập mã vé.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(licenseVehicle))
        {
            message = "Vui lòng nhập biển số xe.";
            return false;
        }

        return _parkingSlotRepository.ConfirmCheckOut(ticketCode.Trim(), licenseVehicle.Trim(), out message);
    }

    public CashCheckoutDto? GetCashCheckout(string ticketCode, string licenseVehicle, out string message)
    {
        if (string.IsNullOrWhiteSpace(ticketCode) || string.IsNullOrWhiteSpace(licenseVehicle))
        {
            message = "Vui lòng nhập đầy đủ mã vé và biển số xe.";
            return null;
        }

        try
        {
            return _parkingSlotRepository.GetCashCheckout(ticketCode.Trim(), licenseVehicle.Trim(), out message);
        }
        catch (System.Exception ex)
        {
            message = $"Không thể tính phí: {ex.GetBaseException().Message}";
            return null;
        }
    }

    public bool CompleteCashCheckout(int sessionId, int staffId, decimal totalAmount, out string message)
    {
        if (sessionId <= 0 || staffId <= 0 || totalAmount < 0)
        {
            message = "Thông tin thanh toán không hợp lệ.";
            return false;
        }

        try
        {
            return _parkingSlotRepository.CompleteCashCheckout(sessionId, staffId, totalAmount, out message);
        }
        catch (System.Exception ex)
        {
            message = $"Không thể hoàn tất thanh toán: {ex.GetBaseException().Message}";
            return false;
        }
    }

    public List<ParkingSession> GetActiveOrReservedSessions()
    {
        return _parkingSlotRepository.GetActiveOrReservedSessions();
    }
}
