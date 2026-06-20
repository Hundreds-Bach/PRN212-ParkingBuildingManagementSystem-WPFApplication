using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PBMS_WPF_Application
{
    /// <summary>
    /// Interaction logic for ParkingMapWindow.xaml
    /// </summary>
    public partial class ParkingMapWindow : Window
    {
        private readonly IParkingSlotService _parkingSlotService;

        public ParkingMapWindow()
        {
            InitializeComponent();
            _parkingSlotService = new ParkingSlotService();
            
            // Set default view on load
            ShowBasement();
        }

        private void LoadParkingSlots(int floorId)
        {
            try
            {
                // Fetch slots from BLL service
                List<ParkingSlot> slots = _parkingSlotService.GetSlotsByFloor(floorId);

                // Set ItemsSource
                lstParkingSlots.ItemsSource = slots;

                // Update text header
                if (floorId == 1)
                {
                    lblFloorHeader.Text = "Đang hiển thị: Basement 1 (Khu vực xe hơi)";
                }
                else
                {
                    lblFloorHeader.Text = "Đang hiển thị: Floor 1 (Khu vực xe máy)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu chỗ đỗ xe: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowBasement()
        {
            // Toggle button styles simply using FontWeights (active is Bold, inactive is Normal)
            btnBasement.FontWeight = FontWeights.Bold;
            btnFloor1.FontWeight = FontWeights.Normal;

            LoadParkingSlots(1);
        }

        private void ShowFloor1()
        {
            // Toggle button styles simply using FontWeights (active is Bold, inactive is Normal)
            btnBasement.FontWeight = FontWeights.Normal;
            btnFloor1.FontWeight = FontWeights.Bold;

            LoadParkingSlots(2);
        }

        private void BtnBasement_Click(object sender, RoutedEventArgs e)
        {
            ShowBasement();
        }

        private void BtnFloor1_Click(object sender, RoutedEventArgs e)
        {
            ShowFloor1();
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
