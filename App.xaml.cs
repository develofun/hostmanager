using System.Windows;
using HostManager.Views;

namespace HostManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 전역 예외 처리
            DispatcherUnhandledException += (sender, args) =>
            {
                ModernMessageBox.Error($"오류가 발생했습니다: {args.Exception.Message}", "오류");
                args.Handled = true;
            };
        }
    }
}
