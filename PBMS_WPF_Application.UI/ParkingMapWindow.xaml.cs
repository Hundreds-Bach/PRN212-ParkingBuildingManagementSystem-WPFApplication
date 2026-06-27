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
        private readonly int _currentUserId;
        private int _currentFloorId = 1;

        public ParkingMapWindow()
            : this(1)
        {
        }

        public ParkingMapWindow(int currentUserId)
        {
            InitializeComponent();
            _parkingSlotService = new ParkingSlotService();
            _currentUserId = currentUserId;
            
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
                lstParkingSlots.SelectedItem = null;
                lblSelectedSlot.Text = "O dang chon: chua co";

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
            _currentFloorId = 1;

            // Toggle button styles simply using FontWeights (active is Bold, inactive is Normal)
            btnBasement.FontWeight = FontWeights.Bold;
            btnFloor1.FontWeight = FontWeights.Normal;

            LoadParkingSlots(1);
        }

        private void ShowFloor1()
        {
            _currentFloorId = 2;

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

        private void LstParkingSlots_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lstParkingSlots.SelectedItem is ParkingSlot slot)
            {
                lblSelectedSlot.Text = $"O dang chon: {slot.SlotName} ({slot.SlotStatus})";
            }
            else
            {
                lblSelectedSlot.Text = "O dang chon: chua co";
            }
        }

        private void BtnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (lstParkingSlots.SelectedItem is not ParkingSlot slot)
            {
                MessageBox.Show("Vui long chon o do xe truoc.", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _parkingSlotService.CheckIn(slot.SlotId, _currentUserId, txtLicensePlate.Text, out string message);
            MessageBox.Show(message, success ? "Thanh cong" : "Thong bao", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (success)
            {
                txtLicensePlate.Text = string.Empty;
                LoadParkingSlots(_currentFloorId);
            }
        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (lstParkingSlots.SelectedItem is not ParkingSlot slot)
            {
                MessageBox.Show("Vui long chon o do xe truoc.", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _parkingSlotService.CheckOut(slot.SlotId, out string message);
            MessageBox.Show(message, success ? "Thanh cong" : "Thong bao", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (success)
            {
                LoadParkingSlots(_currentFloorId);
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
