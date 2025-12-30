using System.Windows;
using System.Windows.Input;

namespace HostManager.Views
{
    /// <summary>
    /// 종료 확인 다이얼로그
    /// </summary>
    public partial class ExitDialog : Window
    {
        public enum ExitAction
        {
            Cancel,
            Exit,
            MinimizeToTray
        }

        public ExitAction SelectedAction { get; private set; } = ExitAction.Cancel;

        public ExitDialog()
        {
            InitializeComponent();
            this.PreviewKeyDown += ExitDialog_PreviewKeyDown;
        }

        private void ExitDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedAction = ExitAction.Cancel;
                this.Close();
                e.Handled = true;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = ExitAction.Exit;
            this.Close();
        }

        private void MinimizeToTrayButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = ExitAction.MinimizeToTray;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = ExitAction.Cancel;
            this.Close();
        }

        public static ExitAction Show(Window owner)
        {
            var dialog = new ExitDialog();
            dialog.Owner = owner;
            dialog.ShowDialog();
            return dialog.SelectedAction;
        }
    }
}
