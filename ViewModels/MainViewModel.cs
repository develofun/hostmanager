using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HostManager.Models;
using HostManager.Resources;
using HostManager.Services;
using HostManager.Views;

namespace HostManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly HostsFileService _hostsService;
        private readonly GroupService _groupService;
        private readonly EnvService _envService;

        private ObservableCollection<HostEntry> _allHostEntries = new();
        private ObservableCollection<HostEntry> _filteredHostEntries = new();
        private ObservableCollection<string> _envList = new();
        private ObservableCollection<string> _groupList = new();
        private string _selectedEnv = "전체";
        private string _selectedGroup = "전체";
        private string _searchText = "";
        private bool _isAllSelected;

        public MainViewModel()
        {
            _hostsService = new HostsFileService();
            _groupService = new GroupService();
            _envService = new EnvService();

            // Commands 초기화
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            SaveCommand = new RelayCommand(ExecuteSave);
            AddCommand = new RelayCommand(ExecuteAdd);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            EnableCommand = new RelayCommand(ExecuteEnable);
            DisableCommand = new RelayCommand(ExecuteDisable);
            GroupCommand = new RelayCommand(ExecuteGroup);
            EnvCommand = new RelayCommand(ExecuteEnv);
            CheckAllCommand = new RelayCommand(ExecuteCheckAll);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ResetFilterCommand = new RelayCommand(ExecuteResetFilter);
            GuideCommand = new RelayCommand(ExecuteGuide);
            BackupCommand = new RelayCommand(ExecuteBackup);
            OpenHostsFileCommand = new RelayCommand(ExecuteOpenHostsFile);

            // 최초 실행 시 기존 hosts 파일 백업
            BackupOriginalHostsFile();

            // 데이터 로드
            LoadData();
        }

        #region Properties

        public ObservableCollection<HostEntry> FilteredHostEntries
        {
            get => _filteredHostEntries;
            set { _filteredHostEntries = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> EnvList
        {
            get => _envList;
            set { _envList = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> GroupList
        {
            get => _groupList;
            set { _groupList = value; OnPropertyChanged(); }
        }

        public string SelectedEnv
        {
            get => _selectedEnv;
            set 
            { 
                _selectedEnv = value; 
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public string SelectedGroup
        {
            get => _selectedGroup;
            set 
            { 
                _selectedGroup = value; 
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set 
            { 
                _searchText = value; 
                OnPropertyChanged();
            }
        }

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                _isAllSelected = value;
                OnPropertyChanged();
                foreach (var entry in FilteredHostEntries)
                {
                    entry.IsSelected = value;
                }
            }
        }

        public ObservableCollection<string> AvailableGroups
        {
            get
            {
                var groups = new ObservableCollection<string> { "", "없음" };
                foreach (var g in GroupList.Where(g => g != "전체"))
                {
                    groups.Add(g);
                }
                return groups;
            }
        }

        public ObservableCollection<string> AvailableEnvs
        {
            get
            {
                var envs = new ObservableCollection<string> { "" };
                foreach (var e in EnvList.Where(e => e != "전체"))
                {
                    envs.Add(e);
                }
                return envs;
            }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EnableCommand { get; }
        public ICommand DisableCommand { get; }
        public ICommand GroupCommand { get; }
        public ICommand EnvCommand { get; }
        public ICommand CheckAllCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand GuideCommand { get; }
        public ICommand BackupCommand { get; }
        public ICommand OpenHostsFileCommand { get; }

        #endregion

        #region Methods

        public void LoadData()
        {
            // 환경 목록 로드
            var envs = _envService.LoadEnvs();
            EnvList = new ObservableCollection<string> { "전체" };
            foreach (var env in envs)
            {
                EnvList.Add(env.Name);
            }

            // 그룹 목록 로드
            var groups = _groupService.LoadGroups();
            GroupList = new ObservableCollection<string> { "전체" };
            foreach (var group in groups)
            {
                GroupList.Add(group.Name);
            }

            // 호스트 목록 로드
            var hosts = _hostsService.LoadHosts();
            foreach (var host in hosts)
            {
                host.MarkAsClean();
            }
            _allHostEntries = new ObservableCollection<HostEntry>(hosts);

            OnPropertyChanged(nameof(AvailableGroups));
            OnPropertyChanged(nameof(AvailableEnvs));

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _allHostEntries.AsEnumerable();

            if (SelectedEnv != "전체" && !string.IsNullOrEmpty(SelectedEnv))
            {
                filtered = filtered.Where(h => h.Env == SelectedEnv);
            }

            if (SelectedGroup != "전체" && !string.IsNullOrEmpty(SelectedGroup))
            {
                filtered = filtered.Where(h => h.Group == SelectedGroup);
            }

            // 검색어 필터링 (IP 또는 호스트에 검색어가 포함된 경우)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(h => 
                    (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchLower)) ||
                    (h.HostName != null && h.HostName.ToLower().Contains(searchLower)));
            }

            FilteredHostEntries = new ObservableCollection<HostEntry>(filtered);
            IsAllSelected = false;
        }

        private void ExecuteSearch(object? parameter)
        {
            ApplyFilter();
        }

        private void ExecuteResetFilter(object? parameter)
        {
            SelectedEnv = "전체";
            SelectedGroup = "전체";
            SearchText = "";
            ApplyFilter();
        }

        private void ExecuteGuide(object? parameter)
        {
            var guideDialog = new Views.GuideDialog();
            guideDialog.Owner = Application.Current.MainWindow;
            guideDialog.ShowDialog();
        }

        private void ExecuteBackup(object? parameter)
        {
            try
            {
                var hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
                var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts_backup");
                
                if (File.Exists(hostsPath))
                {
                    File.Copy(hostsPath, backupPath, true);
                    ModernMessageBox.Success($"{Strings.BackupSuccess}\n{Strings.BackupFile}: {backupPath}", Strings.Success);
                }
                else
                {
                    ModernMessageBox.Error(Strings.HostsFileNotFound, Strings.Error);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ModernMessageBox.Warning($"{Strings.BackupFailed}\n{Strings.AdminRequired}", Strings.PermissionError);
            }
            catch (Exception ex)
            {
                ModernMessageBox.Error($"{Strings.BackupFailed}: {ex.Message}", Strings.Error);
            }
        }

        private void ExecuteOpenHostsFile(object? parameter)
        {
            try
            {
                var hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
                
                if (File.Exists(hostsPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = hostsPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    ModernMessageBox.Error(Strings.HostsFileNotFound, Strings.Error);
                }
            }
            catch (Exception ex)
            {
                ModernMessageBox.Error($"{ex.Message}", Strings.Error);
            }
        }

        private void BackupOriginalHostsFile()
        {
            try
            {
                var hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
                var prevBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts_prev_backup");
                
                if (!File.Exists(hostsPath))
                    return;

                var content = File.ReadAllText(hostsPath);
                
                // Host Manager 형식인지 확인 (메타데이터 태그 존재 여부)
                bool isHostManagerFormat = content.Contains("[Env:") || 
                                           content.Contains("[Group:") || 
                                           content.Contains("[Desc:") ||
                                           content.Contains("# ====================");
                
                // Host Manager 형식이 아니거나 prev_backup이 없는 경우 백업
                if (!isHostManagerFormat || !File.Exists(prevBackupPath))
                {
                    File.Copy(hostsPath, prevBackupPath, true);
                }
            }
            catch
            {
                // 백업 실패해도 프로그램 실행에는 영향 없음
            }
        }

        private void ExecuteRefresh(object? parameter)
        {
            LoadData();
        }

        private void ExecuteSave(object? parameter)
        {
            // IP 주소 유효성 검사
            foreach (var entry in _allHostEntries)
            {
                if (!HostsFileService.IsValidIpAddress(entry.IpAddress))
                {
                    ModernMessageBox.Warning($"{Strings.InvalidIpAddress}: {entry.IpAddress}", Strings.InputError);
                    return;
                }

                if (!HostsFileService.IsValidHostName(entry.HostName))
                {
                    ModernMessageBox.Warning($"{Strings.InvalidHostName}: {entry.HostName}", Strings.InputError);
                    return;
                }
            }

            if (ModernMessageBox.Confirm(Strings.UnsavedChanges, Strings.SaveConfirmTitle))
            {
                try
                {
                    _hostsService.SaveHosts(_allHostEntries.ToList());
                    
                    // 저장 후 모든 항목을 clean 상태로
                    foreach (var entry in _allHostEntries)
                    {
                        entry.MarkAsClean();
                    }
                    
                    ModernMessageBox.Success(Strings.SaveSuccess, Strings.Success);
                }
                catch (Exception ex)
                {
                    ModernMessageBox.Error($"{Strings.SaveFailed}: {ex.Message}", Strings.Error);
                }
            }
        }

        private void ExecuteAdd(object? parameter)
        {
            var dialog = new AddHostDialog(AvailableEnvs.ToList(), AvailableGroups.ToList());
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                var newEntry = dialog.NewHostEntry;
                _allHostEntries.Add(newEntry);
                ApplyFilter();
            }
        }

        private void ExecuteDelete(object? parameter)
        {
            var selectedItems = FilteredHostEntries.Where(h => h.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                ModernMessageBox.Info(Strings.SelectItemToDelete, Strings.Info);
                return;
            }

            if (ModernMessageBox.Confirm($"{Strings.DeleteConfirm} ({selectedItems.Count})", Strings.DeleteConfirmTitle))
            {
                foreach (var item in selectedItems)
                {
                    _allHostEntries.Remove(item);
                }
                ApplyFilter();
            }
        }

        private void ExecuteEnable(object? parameter)
        {
            var selectedItems = FilteredHostEntries.Where(h => h.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                ModernMessageBox.Info(Strings.SelectItemToEnable, Strings.Info);
                return;
            }

            if (ModernMessageBox.Confirm(Strings.Enable + "?", Strings.Confirm))
            {
                foreach (var item in selectedItems)
                {
                    item.IsEnabled = true;
                }
            }
        }

        private void ExecuteDisable(object? parameter)
        {
            var selectedItems = FilteredHostEntries.Where(h => h.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                ModernMessageBox.Info(Strings.SelectItemToDisable, Strings.Info);
                return;
            }

            if (ModernMessageBox.Confirm(Strings.Disable + "?", Strings.Confirm))
            {
                foreach (var item in selectedItems)
                {
                    item.IsEnabled = false;
                }
            }
        }

        private void ExecuteGroup(object? parameter)
        {
            var groups = _groupService.LoadGroups();
            var dialog = new GroupManageDialog(groups);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                var oldGroups = _groupService.LoadGroups().Select(g => g.Name).ToList();
                var newGroups = dialog.Groups;

                // 삭제된 그룹 처리
                var deletedGroups = oldGroups.Except(newGroups.Select(g => g.Name)).ToList();
                foreach (var entry in _allHostEntries)
                {
                    if (deletedGroups.Contains(entry.Group))
                    {
                        entry.Group = string.Empty;
                    }
                }

                // 이름이 변경된 그룹 처리 (순서 기반)
                _groupService.SaveGroups(newGroups.ToList());
                LoadData();
            }
        }

        private void ExecuteEnv(object? parameter)
        {
            var envs = _envService.LoadEnvs();
            var dialog = new EnvManageDialog(envs);
            dialog.Owner = Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true)
            {
                var oldEnvs = _envService.LoadEnvs().Select(e => e.Name).ToList();
                var newEnvs = dialog.Envs;

                // 삭제된 환경 처리
                var deletedEnvs = oldEnvs.Except(newEnvs.Select(e => e.Name)).ToList();
                foreach (var entry in _allHostEntries)
                {
                    if (deletedEnvs.Contains(entry.Env))
                    {
                        entry.Env = string.Empty;
                    }
                }

                _envService.SaveEnvs(newEnvs.ToList());
                LoadData();
            }
        }

        private void ExecuteCheckAll(object? parameter)
        {
            IsAllSelected = !IsAllSelected;
        }

        public void UpdateGroupForEntries(string oldGroupName, string newGroupName)
        {
            foreach (var entry in _allHostEntries)
            {
                if (entry.Group == oldGroupName)
                {
                    entry.Group = newGroupName;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
