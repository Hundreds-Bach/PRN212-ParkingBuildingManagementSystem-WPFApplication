using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;
using System.Linq;
using System.Windows;

namespace PBMS_WPF_Application
{
    public partial class AdminWindow : Window
    {
        private readonly IUserService _userService;
        private readonly int _currentUserId;

        public AdminWindow(int currentUserId)
        {
            InitializeComponent();
            _userService = new UserService();
            _currentUserId = currentUserId;

            LoadRoles();
            LoadUsers();
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

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
            this.Close();
        }
    }
}
