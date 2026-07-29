using PBMS_WPF_Application.BLL.Services;
using System.Windows;

namespace PBMS_WPF_Application
{
    public partial class StaffWindow : Window
    {
        private readonly IParkingSlotService _parkingSlotService;
        private readonly int _currentUserId;

        public StaffWindow(int currentUserId)
        {
            InitializeComponent();
            _parkingSlotService = new ParkingSlotService();
            _currentUserId = currentUserId;

            LoadSessions();
        }

        private void LoadSessions()
        {
            try
            {
                dgSessions.ItemsSource = _parkingSlotService.GetActiveOrReservedSessions();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phiên đỗ xe: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnConfirmCheckIn_Click(object sender, RoutedEventArgs e)
        {
            string ticketCode = txtTicketCode.Text.Trim();
            string licensePlate = txtLicensePlate.Text.Trim();

            if (string.IsNullOrWhiteSpace(ticketCode) || string.IsNullOrWhiteSpace(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã vé và Biển số xe.", "Yêu cầu nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _parkingSlotService.ConfirmCheckIn(ticketCode, licensePlate, out string message);
            MessageBox.Show(message, success ? "Thành Công" : "Lỗi Check-in", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (success)
            {
                txtTicketCode.Text = string.Empty;
                txtLicensePlate.Text = string.Empty;
                LoadSessions();
            }
        }

        private void BtnConfirmCheckOut_Click(object sender, RoutedEventArgs e)
        {
            string ticketCode = txtTicketCode.Text.Trim();
            string licensePlate = txtLicensePlate.Text.Trim();

            if (string.IsNullOrWhiteSpace(ticketCode) || string.IsNullOrWhiteSpace(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã vé và Biển số xe.", "Yêu cầu nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _parkingSlotService.ConfirmCheckOut(ticketCode, licensePlate, out string message);
            MessageBox.Show(message, success ? "Thành Công" : "Lỗi Check-out", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (success)
            {
                txtTicketCode.Text = string.Empty;
                txtLicensePlate.Text = string.Empty;
                LoadSessions();
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
