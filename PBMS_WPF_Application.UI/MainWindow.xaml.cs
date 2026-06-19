using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.Core.DTOs;
using System.Windows;

namespace PBMS_WPF_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IUserService _userService;

        public MainWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void BtnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            // Switch view
            cardLogin.Visibility = Visibility.Collapsed;
            cardRegister.Visibility = Visibility.Visible;

            // Clear errors and inputs
            ClearLoginInputs();
            ClearRegisterInputs();
            borderLoginWarning.Visibility = Visibility.Collapsed;
            borderRegWarning.Visibility = Visibility.Collapsed;
        }

        private void BtnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Switch view
            cardLogin.Visibility = Visibility.Visible;
            cardRegister.Visibility = Visibility.Collapsed;

            // Clear errors and inputs
            ClearLoginInputs();
            ClearRegisterInputs();
            borderLoginWarning.Visibility = Visibility.Collapsed;
            borderRegWarning.Visibility = Visibility.Collapsed;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            borderLoginWarning.Visibility = Visibility.Collapsed;

            var loginDto = new UserLoginDto
            {
                PhoneNumber = txtLoginPhone.Text,
                Password = pbLoginPassword.Password
            };

            var user = _userService.Login(loginDto, out string message);

            if (user != null)
            {
                MessageBox.Show($"Đăng nhập thành công!\nChào mừng {user.Username} (Email: {user.Email}) quay trở lại.", 
                                "Thành Công", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                
                // Keep the credentials cleared
                pbLoginPassword.Password = string.Empty;
            }
            else
            {
                lblLoginWarning.Text = message;
                borderLoginWarning.Visibility = Visibility.Visible;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            borderRegWarning.Visibility = Visibility.Collapsed;

            var registerDto = new UserRegisterDto
            {
                Username = txtRegUsername.Text,
                Email = txtRegEmail.Text,
                PhoneNumber = txtRegPhone.Text,
                Password = pbRegPassword.Password
            };

            bool success = _userService.Register(registerDto, out string message);

            if (success)
            {
                MessageBox.Show(message, "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Clear inputs and redirect to login panel
                ClearRegisterInputs();
                BtnGoToLogin_Click(sender, e);
                
                // Pre-fill phone number in login panel for convenience
                txtLoginPhone.Text = registerDto.PhoneNumber;
            }
            else
            {
                lblRegWarning.Text = message;
                borderRegWarning.Visibility = Visibility.Visible;
            }
        }

        private void ClearLoginInputs()
        {
            txtLoginPhone.Text = string.Empty;
            pbLoginPassword.Password = string.Empty;
        }

        private void ClearRegisterInputs()
        {
            txtRegUsername.Text = string.Empty;
            txtRegEmail.Text = string.Empty;
            txtRegPhone.Text = string.Empty;
            pbRegPassword.Password = string.Empty;
        }
    }
}