using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HostManager.Resources;

namespace HostManager.Views
{
    public enum BulkSetType
    {
        Environment,
        Group
    }

    public partial class BulkSetDialog : Window
    {
        public BulkSetType SetType { get; private set; }
        public string? SelectedValue { get; private set; }
        public int SelectedCount { get; private set; }

        public BulkSetDialog(BulkSetType setType, List<string> availableValues, int selectedCount)
        {
            InitializeComponent();

            SetType = setType;
            SelectedCount = selectedCount;

            // 다국어 적용
            if (setType == BulkSetType.Environment)
            {
                TitleText.Text = Strings.BulkSetEnvTitle;
                ValueLabel.Text = Strings.Environment + ":";
            }
            else
            {
                TitleText.Text = Strings.BulkSetGroupTitle;
                ValueLabel.Text = Strings.Group + ":";
            }

            ItemLabel.Text = Strings.SelectedItems + ":";
            SelectedCountText.Text = string.Format(Strings.ItemCount, selectedCount);
            CancelButton.Content = Strings.Cancel;
            ApplyButton.Content = Strings.Apply;

            // 값 목록 설정
            ValueComboBox.ItemsSource = availableValues;
            if (availableValues.Count > 0)
            {
                ValueComboBox.SelectedIndex = 0;
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                ApplyButton_Click(sender, e);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = ValueComboBox.SelectedItem?.ToString();
            DialogResult = true;
            Close();
        }
    }
}
