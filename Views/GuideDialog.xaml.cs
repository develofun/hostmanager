using System.Windows;
using System.Windows.Input;

namespace HostManager.Views
{
    public partial class GuideDialog : Window
    {
        public GuideDialog()
        {
            InitializeComponent();
            PreviewKeyDown += GuideDialog_PreviewKeyDown;
        }

        private void GuideDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
