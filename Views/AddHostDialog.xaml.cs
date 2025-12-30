using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HostManager.Models;
using HostManager.Services;

namespace HostManager.Views
{
    public partial class AddHostDialog : Window
    {
        public HostEntry NewHostEntry { get; private set; }

        public AddHostDialog(List<string> envs, List<string> groups)
        {
            InitializeComponent();
            PreviewKeyDown += AddHostDialog_PreviewKeyDown;

            EnvComboBox.ItemsSource = envs;
            GroupComboBox.ItemsSource = groups;

            if (envs.Count > 0)
                EnvComboBox.SelectedIndex = 0;
            if (groups.Count > 0)
                GroupComboBox.SelectedIndex = 0;

            NewHostEntry = new HostEntry();
        }

        private void AddHostDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                e.Handled = true;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // IP 주소 유효성 검사
            if (!HostsFileService.IsValidIpAddress(IpAddressTextBox.Text))
            {
                ModernMessageBox.Warning("아이피를 올바르게 입력하세요.", "입력 오류");
                IpAddressTextBox.Focus();
                return;
            }

            // 호스트명 유효성 검사
            if (!HostsFileService.IsValidHostName(HostNameTextBox.Text))
            {
                ModernMessageBox.Warning("호스트명을 올바르게 입력하세요.", "입력 오류");
                HostNameTextBox.Focus();
                return;
            }

            NewHostEntry = new HostEntry
            {
                IpAddress = IpAddressTextBox.Text.Trim(),
                HostName = HostNameTextBox.Text.Trim(),
                Env = EnvComboBox.Text ?? string.Empty,
                Group = GroupComboBox.Text == "없음" ? string.Empty : GroupComboBox.Text ?? string.Empty,
                Description = DescriptionTextBox.Text?.Trim() ?? string.Empty,
                IsEnabled = EnabledCheckBox.IsChecked ?? true,
                IsNew = true
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
