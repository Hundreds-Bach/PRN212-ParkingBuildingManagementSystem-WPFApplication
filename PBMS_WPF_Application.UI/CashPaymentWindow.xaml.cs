using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.Core.DTOs;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PBMS_WPF_Application;

public partial class CashPaymentWindow : Window
{
    private readonly CashCheckoutDto _checkout;
    private readonly int _staffId;
    private readonly IParkingSlotService _service;

    public CashPaymentWindow(CashCheckoutDto checkout, int staffId, IParkingSlotService service)
    {
        InitializeComponent();
        _checkout = checkout;
        _staffId = staffId;
        _service = service;
        lblTicketCode.Text = checkout.TicketCode;
        lblLicensePlate.Text = checkout.LicenseVehicle;
        lblSlot.Text = checkout.SlotName;
        lblVehicleType.Text = checkout.VehicleType;
        lblCheckIn.Text = checkout.CheckInTime.ToString("dd/MM/yyyy HH:mm");
        lblCheckOut.Text = checkout.CheckOutTime.ToString("dd/MM/yyyy HH:mm");
        lblHours.Text = $"{checkout.ChargedHours} giờ × {checkout.PricePerHour:N0} đ";
        lblTotal.Text = $"{checkout.TotalAmount:N0} đ";
        lblChange.Text = "Chưa đủ tiền";
    }

    private bool TryGetCash(out decimal cash)
    {
        string input = txtCashReceived.Text.Trim().Replace(",", "").Replace(".", "");
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out cash);
    }

    private void TxtCashReceived_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (TryGetCash(out decimal cash) && cash >= _checkout.TotalAmount)
        {
            lblChange.Text = $"{cash - _checkout.TotalAmount:N0} đ";
            btnConfirm.IsEnabled = true;
        }
        else
        {
            lblChange.Text = "Chưa đủ tiền";
            btnConfirm.IsEnabled = false;
        }
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        bool success = _service.CompleteCashCheckout(
            _checkout.SessionId, _staffId, _checkout.TotalAmount, out string message);
        MessageBox.Show(message, success ? "Thành công" : "Lỗi thanh toán",
            MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);
        if (success)
        {
            DialogResult = true;
            Close();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
