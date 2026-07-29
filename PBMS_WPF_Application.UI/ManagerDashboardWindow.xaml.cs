using PBMS_WPF_Application.BLL.Services;
using System.Windows;

namespace PBMS_WPF_Application;

public partial class ManagerDashboardWindow : Window
{
    private readonly IManagementService _service = new ManagementService();

    public ManagerDashboardWindow()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var dashboard = _service.GetDashboard();
            txtTotalSlots.Text = dashboard.TotalSlots.ToString();
            txtAvailable.Text = dashboard.AvailableSlots.ToString();
            txtReserved.Text = dashboard.ReservedSlots.ToString();
            txtOccupied.Text = dashboard.OccupiedSlots.ToString();
            txtUsers.Text = dashboard.TotalUsers.ToString();
            txtActive.Text = dashboard.ActiveSessions.ToString();
            txtCompleted.Text = dashboard.CompletedSessions.ToString();
            txtRevenue.Text = $"{dashboard.TotalRevenue:N0} đ";
            dgHistory.ItemsSource = _service.GetParkingHistory();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể tải báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new MainWindow();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }
}
