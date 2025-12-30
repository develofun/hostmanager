using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using HostManager.Resources;
using HostManager.ViewModels;
using HostManager.Views;

namespace HostManager
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private bool _isExiting = false;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Text = "Host Manager";
            
            // 리소스에서 아이콘 로드
            try
            {
                var icoPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Resources", "app.ico");
                if (System.IO.File.Exists(icoPath))
                {
                    _notifyIcon.Icon = new System.Drawing.Icon(icoPath);
                }
                else
                {
                    // exe에서 아이콘 추출 시도
                    var exePath = System.IO.Path.Combine(AppContext.BaseDirectory, "HostManager.exe");
                    if (System.IO.File.Exists(exePath))
                    {
                        _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                    }
                    else
                    {
                        _notifyIcon.Icon = SystemIcons.Application;
                    }
                }
            }
            catch
            {
                // 기본 시스템 아이콘 사용
                _notifyIcon.Icon = SystemIcons.Application;
            }

            // 컨텍스트 메뉴 생성
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            
            var openMenuItem = new System.Windows.Forms.ToolStripMenuItem(Strings.TrayOpen);
            openMenuItem.Click += (s, e) => ShowFromTray();
            contextMenu.Items.Add(openMenuItem);
            
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            
            var exitMenuItem = new System.Windows.Forms.ToolStripMenuItem(Strings.TrayExit);
            exitMenuItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
            
            // 더블클릭으로 창 열기
            _notifyIcon.DoubleClick += (s, e) => ShowFromTray();
        }

        private void ShowFromTray()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Focus();
            _notifyIcon!.Visible = false;
        }

        private void MinimizeToTray()
        {
            this.Hide();
            _notifyIcon!.Visible = true;
            _notifyIcon.ShowBalloonTip(2000, Strings.AppTitle, Strings.TrayMinimized, System.Windows.Forms.ToolTipIcon.Info);
        }

        private void ExitApplication()
        {
            _isExiting = true;
            _notifyIcon?.Dispose();
            _notifyIcon = null;
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                
                var action = ExitDialog.Show(this);
                
                switch (action)
                {
                    case ExitDialog.ExitAction.Exit:
                        ExitApplication();
                        break;
                    case ExitDialog.ExitAction.MinimizeToTray:
                        MinimizeToTray();
                        break;
                    case ExitDialog.ExitAction.Cancel:
                        // 아무것도 하지 않음
                        break;
                }
                
                // 포커스 복원
                this.Activate();
                this.Focus();
            }

            base.OnClosing(e);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // F5 키로 새로고침
            if (e.Key == Key.F5)
            {
                _viewModel.RefreshCommand.Execute(null);
                e.Handled = true;
            }
            // Ctrl+F: 검색 InputBox 포커스
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SearchTextBox.Focus();
                SearchTextBox.SelectAll();
                e.Handled = true;
            }
            // Ctrl+1: 환경 DropBox 펼치기/접기
            else if (e.Key == Key.D1 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                EnvComboBox.IsDropDownOpen = !EnvComboBox.IsDropDownOpen;
                e.Handled = true;
            }
            // Ctrl+2: 그룹 DropBox 펼치기/접기
            else if (e.Key == Key.D2 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                GroupComboBox.IsDropDownOpen = !GroupComboBox.IsDropDownOpen;
                e.Handled = true;
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.SearchCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
