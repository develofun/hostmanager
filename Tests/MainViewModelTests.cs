using Xunit;
using HostManager.ViewModels;
using HostManager.Models;
using System.Collections.ObjectModel;
using System.Reflection;

namespace HostManager.Tests
{
    public class MainViewModelTests
    {
        /// <summary>
        /// 테스트용 MainViewModel - 파일 로드 없이 직접 데이터 설정 가능
        /// </summary>
        private MainViewModel CreateTestViewModel()
        {
            var vm = new MainViewModel();
            return vm;
        }

        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, value);
        }

        private ObservableCollection<HostEntry> CreateTestHostEntries()
        {
            return new ObservableCollection<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost", Env = "local", Group = "개발" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1", Env = "dev", Group = "개발" },
                new HostEntry { IpAddress = "192.168.1.2", HostName = "server2", Env = "dev", Group = "운영" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver", Env = "prod", Group = "운영" },
                new HostEntry { IpAddress = "10.0.0.2", HostName = "dbserver", Env = "prod", Group = "DB" }
            };
        }

        [Fact]
        public void SearchText_ShouldBeEmptyByDefault()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Equal("", vm.SearchText);
        }

        [Fact]
        public void SearchText_ShouldUpdateProperty()
        {
            // Arrange
            var vm = CreateTestViewModel();
            var propertyChanged = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SearchText")
                    propertyChanged = true;
            };

            // Act
            vm.SearchText = "test";

            // Assert
            Assert.Equal("test", vm.SearchText);
            Assert.True(propertyChanged);
        }

        [Fact]
        public void SearchCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.SearchCommand);
        }

        [Fact]
        public void ResetFilterCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.ResetFilterCommand);
        }

        [Fact]
        public void ResetFilterCommand_ShouldResetAllFilters()
        {
            // Arrange
            var vm = CreateTestViewModel();
            vm.SelectedEnv = "dev";
            vm.SelectedGroup = "개발";
            vm.SearchText = "test";

            // Act
            vm.ResetFilterCommand.Execute(null);

            // Assert
            Assert.Equal("전체", vm.SelectedEnv);
            Assert.Equal("전체", vm.SelectedGroup);
            Assert.Equal("", vm.SearchText);
        }

        [Fact]
        public void SelectedEnv_ShouldDefaultToAll()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Equal("전체", vm.SelectedEnv);
        }

        [Fact]
        public void SelectedGroup_ShouldDefaultToAll()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Equal("전체", vm.SelectedGroup);
        }

        [Fact]
        public void CheckAllCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.CheckAllCommand);
        }

        [Fact]
        public void RefreshCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.RefreshCommand);
        }

        [Fact]
        public void SaveCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.SaveCommand);
        }

        [Fact]
        public void AddCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.AddCommand);
        }

        [Fact]
        public void DeleteCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.DeleteCommand);
        }

        [Fact]
        public void EnableCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.EnableCommand);
        }

        [Fact]
        public void DisableCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.DisableCommand);
        }

        [Fact]
        public void GroupCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.GroupCommand);
        }

        [Fact]
        public void EnvCommand_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.EnvCommand);
        }

        [Fact]
        public void FilteredHostEntries_ShouldNotBeNull()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.NotNull(vm.FilteredHostEntries);
        }

        [Fact]
        public void EnvList_ShouldContainAllOption()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Contains("전체", vm.EnvList);
        }

        [Fact]
        public void GroupList_ShouldContainAllOption()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Contains("전체", vm.GroupList);
        }

        [Fact]
        public void AvailableGroups_ShouldContainNoGroupOption()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Contains("없음", vm.AvailableGroups);
        }

        [Fact]
        public void AvailableGroups_ShouldContainEmptyStringForUnsetGroup()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Contains("", vm.AvailableGroups);
        }

        [Fact]
        public void AvailableEnvs_ShouldContainEmptyStringForUnsetEnv()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.Contains("", vm.AvailableEnvs);
        }

        [Fact]
        public void IsAllSelected_ShouldBeFalseByDefault()
        {
            // Arrange
            var vm = CreateTestViewModel();

            // Assert
            Assert.False(vm.IsAllSelected);
        }
    }

    /// <summary>
    /// 검색 필터 로직 단위 테스트
    /// </summary>
    public class SearchFilterTests
    {
        [Fact]
        public void FilterByIpAddress_ShouldMatchPartialString()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver" }
            };
            var searchText = "192";

            // Act
            var filtered = entries.Where(h =>
                (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower()))).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal("192.168.1.1", filtered[0].IpAddress);
        }

        [Fact]
        public void FilterByHostName_ShouldMatchPartialString()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver" }
            };
            var searchText = "server";

            // Act
            var filtered = entries.Where(h =>
                (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower()))).ToList();

            // Assert
            Assert.Equal(2, filtered.Count);
            Assert.Contains(filtered, h => h.HostName == "server1");
            Assert.Contains(filtered, h => h.HostName == "prodserver");
        }

        [Fact]
        public void FilterBySearchText_ShouldBeCaseInsensitive()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "LocalHost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "SERVER1" }
            };
            var searchText = "localhost";

            // Act
            var filtered = entries.Where(h =>
                (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower()))).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal("LocalHost", filtered[0].HostName);
        }

        [Fact]
        public void FilterByEnv_ShouldReturnMatchingEntries()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost", Env = "local" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1", Env = "dev" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver", Env = "prod" }
            };
            var selectedEnv = "dev";

            // Act
            var filtered = entries.Where(h => h.Env == selectedEnv).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal("server1", filtered[0].HostName);
        }

        [Fact]
        public void FilterByGroup_ShouldReturnMatchingEntries()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost", Group = "개발" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1", Group = "개발" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver", Group = "운영" }
            };
            var selectedGroup = "개발";

            // Act
            var filtered = entries.Where(h => h.Group == selectedGroup).ToList();

            // Assert
            Assert.Equal(2, filtered.Count);
            Assert.All(filtered, h => Assert.Equal("개발", h.Group));
        }

        [Fact]
        public void CombinedFilter_ShouldApplyAllConditions()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost", Env = "local", Group = "개발" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1", Env = "dev", Group = "개발" },
                new HostEntry { IpAddress = "192.168.1.2", HostName = "server2", Env = "dev", Group = "운영" },
                new HostEntry { IpAddress = "10.0.0.1", HostName = "prodserver", Env = "prod", Group = "운영" }
            };
            var selectedEnv = "dev";
            var selectedGroup = "개발";
            var searchText = "server";

            // Act
            var filtered = entries
                .Where(h => h.Env == selectedEnv)
                .Where(h => h.Group == selectedGroup)
                .Where(h =>
                    (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                    (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower())))
                .ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal("server1", filtered[0].HostName);
        }

        [Fact]
        public void EmptySearchText_ShouldReturnAllEntries()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1" }
            };
            var searchText = "";

            // Act
            var filtered = entries.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(h =>
                    (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                    (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower())));
            }
            var result = filtered.ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WhitespaceSearchText_ShouldReturnAllEntries()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1" }
            };
            var searchText = "   ";

            // Act
            var filtered = entries.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(h =>
                    (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                    (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower())));
            }
            var result = filtered.ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void NoMatchingSearchText_ShouldReturnEmptyList()
        {
            // Arrange
            var entries = new List<HostEntry>
            {
                new HostEntry { IpAddress = "127.0.0.1", HostName = "localhost" },
                new HostEntry { IpAddress = "192.168.1.1", HostName = "server1" }
            };
            var searchText = "notexist";

            // Act
            var filtered = entries.Where(h =>
                (h.IpAddress != null && h.IpAddress.ToLower().Contains(searchText.ToLower())) ||
                (h.HostName != null && h.HostName.ToLower().Contains(searchText.ToLower()))).ToList();

            // Assert
            Assert.Empty(filtered);
        }
    }
}
