# Host Manager

A modern Windows hosts file management tool built with WPF (.NET 8).

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 Overview

Host Manager is a GUI application that helps you easily manage the Windows hosts file (`C:\Windows\System32\drivers\etc\hosts`). It supports environment/group-based classification, bulk enable/disable operations, and a modern UI design.

## ✨ Features

- **Host Management**: Add, edit, delete host entries
- **Environment Classification**: Organize hosts by environment (local, dev, staging, prod, etc.)
- **Group Classification**: Organize hosts by group (API, DB, Web, etc.)
- **Enable/Disable Toggle**: Quickly enable or disable hosts without deleting them
- **Search & Filter**: Filter hosts by environment, group, or search keywords
- **Change Highlighting**: Visual indicators for modified (yellow) and new (green) entries
- **Auto Backup**: Automatically backs up original hosts file on first run (`hosts_prev_backup`)
- **Manual Backup**: Create manual backups anytime (`hosts_backup`)
- **System Tray**: Minimize to system tray for background operation
- **Localization**: Supports English and Korean based on system language

## 🚀 Installation

### Option 1: Download Release
1. Download `HostManager.exe` from the [Releases](../../releases) page
2. Run as Administrator (required to modify hosts file)

### Option 2: Build from Source
```bash
# Clone repository
git clone https://github.com/yourusername/HostManager.git
cd HostManager

# Build
dotnet build

# Run
dotnet run

# Publish (single file)
dotnet publish -c Release -o publish
```

## 📖 Usage

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl + H` | Open Guide |
| `Ctrl + R` / `F5` | Refresh |
| `Ctrl + S` | Save |
| `Ctrl + N` | Add New Host |
| `Delete` | Delete Selected |
| `Ctrl + A` | Select All/None |
| `Ctrl + F` | Focus Search Box |
| `Ctrl + 1` | Toggle Environment Dropdown |
| `Ctrl + 2` | Toggle Group Dropdown |
| `ESC` | Close Popup |

### Basic Workflow

1. **Add Host**: Click "Add" button or press `Ctrl+N`
2. **Edit Host**: Click on any field in the table to edit directly
3. **Save Changes**: Click "Save" button or press `Ctrl+S`
4. **Enable/Disable**: Select hosts with checkboxes, then click "Enable" or "Disable"
5. **Filter**: Use environment/group dropdowns or search box to filter hosts

### Hosts File Format

Host Manager uses a specific format to store metadata:

```
# ==================== [local] ====================
# --- API ---
127.0.0.1 local.api.com # [Env:local] [Group:API] [Desc:Local API Server]
#127.0.0.1 local.db.com # [Env:local] [Group:DB] [Desc:Disabled]

# ==================== [dev] ====================
# --- Web ---
192.168.1.100 dev.myapp.com # [Env:dev] [Group:Web] [Desc:Dev Server]
```

### Backup Files

| File | Description |
|------|-------------|
| `hosts_prev_backup` | Auto-created on first run (preserves original format) |
| `hosts_backup` | Created manually via "Backup" button |

Location: `C:\Windows\System32\drivers\etc\`

## ⚠️ Important Notes

- **Administrator privileges required**: The program must run as Administrator to modify the hosts file
- **Backup recommended**: Always backup before making significant changes
- **Format conversion**: Host Manager converts hosts files to its own format for metadata support

## 🛠️ Development

### Requirements
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio 2022 or VS Code

### Project Structure
```
HostManager/
├── Models/          # Data models
├── Views/           # WPF views (XAML)
├── ViewModels/      # MVVM ViewModels
├── Services/        # Business logic services
├── Converters/      # WPF value converters
├── Resources/       # Localization resources
├── Styles/          # WPF styles
└── Tests/           # Unit tests
```

### Running Tests
```bash
cd Tests
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

[한국어 README](README.ko.md)
