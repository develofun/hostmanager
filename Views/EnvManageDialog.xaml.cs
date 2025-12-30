using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HostManager.Models;

namespace HostManager.Views
{
    public partial class EnvManageDialog : Window
    {
        public ObservableCollection<HostEnv> Envs { get; private set; }

        public EnvManageDialog(List<HostEnv> envs)
        {
            InitializeComponent();
            PreviewKeyDown += EnvManageDialog_PreviewKeyDown;
            Envs = new ObservableCollection<HostEnv>(envs.Select(e => new HostEnv 
            { 
                Name = e.Name, 
                IsDefault = e.IsDefault 
            }));
            EnvListView.ItemsSource = Envs;
        }

        private void EnvManageDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = true;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newEnv = new HostEnv { Name = "", IsEditing = true, IsDefault = false };
            Envs.Add(newEnv);

            // 스크롤을 아래로 이동하고 포커스 설정
            EnvListView.ScrollIntoView(newEnv);
            EnvListView.UpdateLayout();

            var container = EnvListView.ItemContainerGenerator.ContainerFromItem(newEnv) as ListViewItem;
            if (container != null)
            {
                var textBox = FindVisualChild<TextBox>(container);
                if (textBox != null)
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var env = button?.DataContext as HostEnv;
            if (env != null)
            {
                if (ModernMessageBox.Confirm($"'{env.Name}' 환경을 삭제하시겠습니까?\n해당 환경으로 설정된 호스트의 환경이 제거됩니다.", "삭제 확인"))
                {
                    Envs.Remove(env);
                }
            }
        }

        private void NameText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var env = textBlock?.DataContext as HostEnv;
            if (env != null)
            {
                env.IsEditing = true;
                
                // 편집 모드로 전환 후 TextBox에 포커스
                EnvListView.UpdateLayout();
                var container = EnvListView.ItemContainerGenerator.ContainerFromItem(env) as ListViewItem;
                if (container != null)
                {
                    var textBox = FindVisualChild<TextBox>(container);
                    if (textBox != null)
                    {
                        textBox.Focus();
                        textBox.SelectAll();
                    }
                }
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var env = textBox?.DataContext as HostEnv;
            if (env != null)
            {
                // 빈 이름이면 삭제
                if (string.IsNullOrWhiteSpace(env.Name))
                {
                    Envs.Remove(env);
                }
                else
                {
                    env.IsEditing = false;
                }
            }
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                var env = textBox?.DataContext as HostEnv;
                if (env != null)
                {
                    if (string.IsNullOrWhiteSpace(env.Name))
                    {
                        Envs.Remove(env);
                    }
                    else
                    {
                        env.IsEditing = false;
                    }
                }
                // 포커스를 다른 곳으로 이동
                EnvListView.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                var env = textBox?.DataContext as HostEnv;
                if (env != null)
                {
                    Envs.Remove(env);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 편집 중인 빈 항목 제거
            var emptyEnvs = Envs.Where(env => string.IsNullOrWhiteSpace(env.Name)).ToList();
            foreach (var env in emptyEnvs)
            {
                Envs.Remove(env);
            }

            DialogResult = true;
            Close();
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T found)
                    return found;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
