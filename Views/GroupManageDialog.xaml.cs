using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HostManager.Models;

namespace HostManager.Views
{
    public partial class GroupManageDialog : Window
    {
        public ObservableCollection<HostGroup> Groups { get; private set; }

        public GroupManageDialog(List<HostGroup> groups)
        {
            InitializeComponent();
            PreviewKeyDown += GroupManageDialog_PreviewKeyDown;
            Groups = new ObservableCollection<HostGroup>(groups.Select(g => new HostGroup { Name = g.Name }));
            GroupListView.ItemsSource = Groups;
        }

        private void GroupManageDialog_PreviewKeyDown(object sender, KeyEventArgs e)
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
            var newGroup = new HostGroup { Name = "", IsEditing = true };
            Groups.Add(newGroup);

            // 스크롤을 아래로 이동하고 포커스 설정
            GroupListView.ScrollIntoView(newGroup);
            GroupListView.UpdateLayout();

            var container = GroupListView.ItemContainerGenerator.ContainerFromItem(newGroup) as ListViewItem;
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
            var group = button?.DataContext as HostGroup;
            if (group != null)
            {
                if (ModernMessageBox.Confirm($"'{group.Name}' 그룹을 삭제하시겠습니까?\n해당 그룹으로 설정된 호스트의 그룹이 제거됩니다.", "삭제 확인"))
                {
                    Groups.Remove(group);
                }
            }
        }

        private void NameText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var group = textBlock?.DataContext as HostGroup;
            if (group != null)
            {
                group.IsEditing = true;
                
                // 편집 모드로 전환 후 TextBox에 포커스
                GroupListView.UpdateLayout();
                var container = GroupListView.ItemContainerGenerator.ContainerFromItem(group) as ListViewItem;
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
            var group = textBox?.DataContext as HostGroup;
            if (group != null)
            {
                // 빈 이름이면 삭제
                if (string.IsNullOrWhiteSpace(group.Name))
                {
                    Groups.Remove(group);
                }
                else
                {
                    group.IsEditing = false;
                }
            }
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                var group = textBox?.DataContext as HostGroup;
                if (group != null)
                {
                    if (string.IsNullOrWhiteSpace(group.Name))
                    {
                        Groups.Remove(group);
                    }
                    else
                    {
                        group.IsEditing = false;
                    }
                }
                // 포커스를 다른 곳으로 이동
                GroupListView.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                var group = textBox?.DataContext as HostGroup;
                if (group != null)
                {
                    Groups.Remove(group);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 편집 중인 빈 항목 제거
            var emptyGroups = Groups.Where(g => string.IsNullOrWhiteSpace(g.Name)).ToList();
            foreach (var g in emptyGroups)
            {
                Groups.Remove(g);
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
