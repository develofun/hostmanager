using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HostManager.Views
{
    public enum MessageType
    {
        Info,
        Success,
        Warning,
        Error,
        Question
    }

    public enum MessageButtons
    {
        OK,
        YesNo,
        OKCancel
    }

    public partial class ModernMessageBox : Window
    {
        public bool Result { get; private set; }
        private Window? _ownerWindow;

        public ModernMessageBox()
        {
            InitializeComponent();
            PreviewKeyDown += ModernMessageBox_PreviewKeyDown;
            Closed += ModernMessageBox_Closed;
        }

        private void ModernMessageBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = false;
                Close();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Result = true;
                Close();
                e.Handled = true;
            }
        }

        private void ModernMessageBox_Closed(object? sender, EventArgs e)
        {
            // 닫힌 후 부모 윈도우에 포커스 복원
            if (_ownerWindow != null && _ownerWindow.IsVisible)
            {
                _ownerWindow.Activate();
                _ownerWindow.Focus();
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
            Result = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        /// <summary>
        /// Modern UI 스타일 메시지 박스 표시
        /// </summary>
        public static bool Show(string message, string title = "알림", MessageType type = MessageType.Info, MessageButtons buttons = MessageButtons.OK)
        {
            var dialog = new ModernMessageBox();
            dialog.MessageText.Text = message;
            dialog.TitleText.Text = title;

            // 메시지 타입에 따른 스타일 설정
            switch (type)
            {
                case MessageType.Success:
                    dialog.IconText.Text = "✅";
                    dialog.TitleBorder.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
                    break;
                case MessageType.Warning:
                    dialog.IconText.Text = "⚠️";
                    dialog.TitleBorder.Background = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                    break;
                case MessageType.Error:
                    dialog.IconText.Text = "❌";
                    dialog.TitleBorder.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
                    break;
                case MessageType.Question:
                    dialog.IconText.Text = "❓";
                    dialog.TitleBorder.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue
                    break;
                default: // Info
                    dialog.IconText.Text = "ℹ️";
                    dialog.TitleBorder.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue
                    break;
            }

            // 버튼 설정
            switch (buttons)
            {
                case MessageButtons.YesNo:
                    dialog.OkButton.Content = "예";
                    dialog.CancelButton.Content = "아니오";
                    dialog.CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageButtons.OKCancel:
                    dialog.OkButton.Content = "확인";
                    dialog.CancelButton.Content = "취소";
                    dialog.CancelButton.Visibility = Visibility.Visible;
                    break;
                default: // OK
                    dialog.OkButton.Content = "확인";
                    dialog.CancelButton.Visibility = Visibility.Collapsed;
                    break;
            }

            // 부모 윈도우 설정
            if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible)
            {
                dialog.Owner = Application.Current.MainWindow;
                dialog._ownerWindow = Application.Current.MainWindow;
            }

            dialog.ShowDialog();
            return dialog.Result;
        }

        /// <summary>
        /// 확인 메시지 (예/아니오)
        /// </summary>
        public static bool Confirm(string message, string title = "확인")
        {
            return Show(message, title, MessageType.Question, MessageButtons.YesNo);
        }

        /// <summary>
        /// 정보 메시지
        /// </summary>
        public static void Info(string message, string title = "알림")
        {
            Show(message, title, MessageType.Info, MessageButtons.OK);
        }

        /// <summary>
        /// 성공 메시지
        /// </summary>
        public static void Success(string message, string title = "완료")
        {
            Show(message, title, MessageType.Success, MessageButtons.OK);
        }

        /// <summary>
        /// 경고 메시지
        /// </summary>
        public static void Warning(string message, string title = "경고")
        {
            Show(message, title, MessageType.Warning, MessageButtons.OK);
        }

        /// <summary>
        /// 오류 메시지
        /// </summary>
        public static void Error(string message, string title = "오류")
        {
            Show(message, title, MessageType.Error, MessageButtons.OK);
        }
    }
}
