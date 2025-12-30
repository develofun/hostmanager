using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HostManager.Models
{
    public class HostGroup : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isEditing;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
