using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PBMS_WPF_Application
{
    public partial class AdminWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IManagementService _managementService;
        private readonly int _currentUserId;
        private int? _editingFloorId;
        private int? _editingSlotId;

        public AdminWindow(int currentUserId)
        {
            InitializeComponent();
            _userService = new UserService();
            _managementService = new ManagementService();
            _currentUserId = currentUserId;

            LoadRoles();
            LoadUsers();
            LoadManagementData();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _userService.GetAllRoles()
                    .Where(r => r.RoleId != 1) // Do not allow creating new Registered_Drivers directly here
                    .ToList();
                cbRoles.ItemsSource = roles;
                cbRoles.DisplayMemberPath = "RoleName";
                cbRoles.SelectedValuePath = "RoleId";
                if (roles.Count > 0)
                {
                    cbRoles.SelectedIndex = 0;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách vai trò: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = _userService.GetAllUsers();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách người dùng: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadManagementData()
        {
            try
            {
                var floors = _managementService.GetFloors();
                var types = _managementService.GetVehicleTypes();
                dgFloors.ItemsSource = floors;
                dgSlots.ItemsSource = _managementService.GetSlots();
                dgPrices.ItemsSource = types;
                cboFloors.ItemsSource = floors.Where(f => !f.IsDeleted).ToList();
                cboTypes.ItemsSource = types;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu quản trị: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowManagementResult(bool success, string message)
        {
            MessageBox.Show(message, success ? "Thành công" : "Thông báo", MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Warning);
            if (success) LoadManagementData();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRoles();
            LoadUsers();
            LoadManagementData();
        }

        private void BtnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            if (cbRoles.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn vai trò cho người dùng.", "Yêu cầu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedRoleId = (int)cbRoles.SelectedValue;

            var registerDto = new UserRegisterDto
            {
                Username = txtUsername.Text,
                Email = txtEmail.Text,
                PhoneNumber = txtPhone.Text,
                Password = pbPassword.Password
            };

            bool success = _userService.CreateUserByAdmin(registerDto, selectedRoleId, out string message);
            MessageBox.Show(message, success ? "Thành Công" : "Lỗi Tạo Tài Khoản", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (success)
            {
                ClearInputs();
                LoadUsers();
            }
        }

        private void ClearInputs()
        {
            txtUsername.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            pbPassword.Password = string.Empty;
            if (cbRoles.Items.Count > 0)
            {
                cbRoles.SelectedIndex = 0;
            }
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
                ShowManagementResult(false, "Sức chứa phải là số nguyên.");
                return;
            }
            bool success = _managementService.SaveFloor(_editingFloorId, txtFloorName.Text, capacity, out string message);
            ShowManagementResult(success, message);
        }

        private void DgSlots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSlots.SelectedItem is not ParkingSlot slot) return;
            _editingSlotId = slot.SlotId;
            txtSlotName.Text = slot.SlotName;
            cboFloors.SelectedValue = slot.FloorId;
            cboTypes.SelectedValue = slot.TypeId;
            foreach (ComboBoxItem item in cboStatus.Items)
            {
                if (string.Equals(item.Content?.ToString(), slot.SlotStatus, System.StringComparison.OrdinalIgnoreCase))
                {
                    cboStatus.SelectedItem = item;
                    break;
                }
            }
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
                ShowManagementResult(false, "Vui lòng chọn tầng và loại xe.");
                return;
            }
            string status = (cboStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "available";
            bool success = _managementService.SaveSlot(_editingSlotId, txtSlotName.Text, floorId, typeId, status, out string message);
            ShowManagementResult(success, message);
        }

        private void BtnToggleSlot_Click(object sender, RoutedEventArgs e)
        {
            if (dgSlots.SelectedItem is not ParkingSlot slot) return;
            bool success = _managementService.ToggleSlot(slot.SlotId, !slot.IsDeleted, out string message);
            ShowManagementResult(success, message);
        }

        private void DgPrices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPrices.SelectedItem is VehiclesType type) txtPrice.Text = type.Price.ToString("0.##");
        }

        private void BtnUpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            if (dgPrices.SelectedItem is not VehiclesType type || !decimal.TryParse(txtPrice.Text, out decimal price))
            {
                ShowManagementResult(false, "Vui lòng chọn loại xe và nhập giá hợp lệ.");
                return;
            }
            bool success = _managementService.UpdateVehiclePrice(type.TypeId, price, out string message);
            ShowManagementResult(success, message);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
            this.Close();
        }
    }
}
