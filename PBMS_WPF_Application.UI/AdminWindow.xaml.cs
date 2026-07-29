using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.DAL.Entities;
using System.Windows;
using System.Windows.Controls;

namespace PBMS_WPF_Application;

public partial class AdminWindow : Window
{
    private readonly IManagementService _service = new ManagementService();
    private readonly int _currentAdminId;
    private int? _editingFloorId;
    private int? _editingSlotId;

    public AdminWindow(int currentAdminId)
    {
        InitializeComponent();
        _currentAdminId = currentAdminId;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var floors = _service.GetFloors();
            var types = _service.GetVehicleTypes();
            dgUsers.ItemsSource = _service.GetUsers();
            dgFloors.ItemsSource = floors;
            dgSlots.ItemsSource = _service.GetSlots();
            dgPrices.ItemsSource = types;
            cboRoles.ItemsSource = _service.GetRoles();
            cboFloors.ItemsSource = floors.Where(f => !f.IsDeleted).ToList();
            cboTypes.ItemsSource = types;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải dữ liệu quản trị: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowResult(bool success, string message)
    {
        MessageBox.Show(message, success ? "Thành công" : "Thông báo", MessageBoxButton.OK,
            success ? MessageBoxImage.Information : MessageBoxImage.Warning);
        if (success) LoadData();
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

    private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgUsers.SelectedItem is User user) cboRoles.SelectedValue = user.RoleId;
    }

    private void BtnUpdateRole_Click(object sender, RoutedEventArgs e)
    {
        if (dgUsers.SelectedItem is not User user || cboRoles.SelectedValue is not int roleId) return;
        bool success = _service.UpdateUserRole(_currentAdminId, user.UserId, roleId, out string message);
        ShowResult(success, message);
    }

    private void BtnToggleUser_Click(object sender, RoutedEventArgs e)
    {
        if (dgUsers.SelectedItem is not User user) return;
        bool success = _service.ToggleUser(_currentAdminId, user.UserId, !user.IsDeleted, out string message);
        ShowResult(success, message);
    }

    private void DgFloors_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgFloors.SelectedItem is not Floor floor) return;
        _editingFloorId = floor.FloorId;
        txtFloorName.Text = floor.FloorName;
        txtCapacity.Text = floor.Capacity.ToString();
    }

    private void BtnNewFloor_Click(object sender, RoutedEventArgs e)
    {
        _editingFloorId = null;
        dgFloors.SelectedItem = null;
        txtFloorName.Clear();
        txtCapacity.Clear();
    }

    private void BtnSaveFloor_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(txtCapacity.Text, out int capacity))
        {
            ShowResult(false, "Sức chứa phải là số nguyên.");
            return;
        }
        bool success = _service.SaveFloor(_editingFloorId, txtFloorName.Text, capacity, out string message);
        ShowResult(success, message);
    }

    private void DgSlots_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgSlots.SelectedItem is not ParkingSlot slot) return;
        _editingSlotId = slot.SlotId;
        txtSlotName.Text = slot.SlotName;
        cboFloors.SelectedValue = slot.FloorId;
        cboTypes.SelectedValue = slot.TypeId;
        foreach (ComboBoxItem item in cboStatus.Items)
            if (string.Equals(item.Content?.ToString(), slot.SlotStatus, StringComparison.OrdinalIgnoreCase))
                cboStatus.SelectedItem = item;
    }

    private void BtnNewSlot_Click(object sender, RoutedEventArgs e)
    {
        _editingSlotId = null;
        dgSlots.SelectedItem = null;
        txtSlotName.Clear();
        cboFloors.SelectedIndex = 0;
        cboTypes.SelectedIndex = 0;
        cboStatus.SelectedIndex = 0;
    }

    private void BtnSaveSlot_Click(object sender, RoutedEventArgs e)
    {
        if (cboFloors.SelectedValue is not int floorId || cboTypes.SelectedValue is not int typeId)
        {
            ShowResult(false, "Vui lòng chọn tầng và loại xe.");
            return;
        }
        string status = (cboStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "available";
        bool success = _service.SaveSlot(_editingSlotId, txtSlotName.Text, floorId, typeId, status, out string message);
        ShowResult(success, message);
    }

    private void BtnToggleSlot_Click(object sender, RoutedEventArgs e)
    {
        if (dgSlots.SelectedItem is not ParkingSlot slot) return;
        bool success = _service.ToggleSlot(slot.SlotId, !slot.IsDeleted, out string message);
        ShowResult(success, message);
    }

    private void DgPrices_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgPrices.SelectedItem is VehiclesType type) txtPrice.Text = type.Price.ToString("0.##");
    }

    private void BtnUpdatePrice_Click(object sender, RoutedEventArgs e)
    {
        if (dgPrices.SelectedItem is not VehiclesType type || !decimal.TryParse(txtPrice.Text, out decimal price))
        {
            ShowResult(false, "Vui lòng chọn loại xe và nhập giá hợp lệ.");
            return;
        }
        bool success = _service.UpdateVehiclePrice(type.TypeId, price, out string message);
        ShowResult(success, message);
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new MainWindow();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }
}
