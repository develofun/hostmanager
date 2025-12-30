using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HostManager.Models
{
    public class HostEntry : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _ipAddress = string.Empty;
        private string _hostName = string.Empty;
        private string _env = string.Empty;
        private string _group = string.Empty;
        private string _description = string.Empty;
        private bool _isEnabled;
        private int _originalLineNumber;
        private bool _isDirty;

        // 원본 값 저장용 (변경 감지)
        private string _originalIpAddress = string.Empty;
        private string _originalHostName = string.Empty;
        private string _originalEnv = string.Empty;
        private string _originalGroup = string.Empty;
        private string _originalDescription = string.Empty;
        private bool _originalIsEnabled;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); CheckDirty(); }
        }

        public string HostName
        {
            get => _hostName;
            set { _hostName = value; OnPropertyChanged(); CheckDirty(); }
        }

        public string Env
        {
            get => _env;
            set { _env = value; OnPropertyChanged(); CheckDirty(); }
        }

        public string Group
        {
            get => _group;
            set { _group = value; OnPropertyChanged(); CheckDirty(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); CheckDirty(); }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); CheckDirty(); }
        }

        public string Status => IsEnabled ? "Enabled" : "Disabled";

        public int OriginalLineNumber
        {
            get => _originalLineNumber;
            set { _originalLineNumber = value; OnPropertyChanged(); }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set { _isDirty = value; OnPropertyChanged(); }
        }

        public bool IsNew { get; set; }

        private void CheckDirty()
        {
            if (IsNew)
            {
                IsDirty = true;
                return;
            }

            IsDirty = _ipAddress != _originalIpAddress ||
                      _hostName != _originalHostName ||
                      _env != _originalEnv ||
                      _group != _originalGroup ||
                      _description != _originalDescription ||
                      _isEnabled != _originalIsEnabled;
        }

        public void MarkAsClean()
        {
            _originalIpAddress = _ipAddress;
            _originalHostName = _hostName;
            _originalEnv = _env;
            _originalGroup = _group;
            _originalDescription = _description;
            _originalIsEnabled = _isEnabled;
            IsNew = false;
            IsDirty = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HostEntry Clone()
        {
            return new HostEntry
            {
                IsSelected = this.IsSelected,
                IpAddress = this.IpAddress,
                HostName = this.HostName,
                Env = this.Env,
                Group = this.Group,
                Description = this.Description,
                IsEnabled = this.IsEnabled,
                OriginalLineNumber = this.OriginalLineNumber
            };
        }
    }
}
