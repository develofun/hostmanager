using Xunit;
using HostManager.Models;

namespace HostManager.Tests
{
    public class HostEntryModelTests
    {
        [Fact]
        public void HostEntry_Status_ShouldReturnEnabled_WhenIsEnabledTrue()
        {
            // Arrange
            var entry = new HostEntry { IsEnabled = true };

            // Act & Assert
            Assert.Equal("Enabled", entry.Status);
        }

        [Fact]
        public void HostEntry_Status_ShouldReturnDisabled_WhenIsEnabledFalse()
        {
            // Arrange
            var entry = new HostEntry { IsEnabled = false };

            // Act & Assert
            Assert.Equal("Disabled", entry.Status);
        }

        [Fact]
        public void HostEntry_Clone_ShouldCreateDeepCopy()
        {
            // Arrange
            var original = new HostEntry
            {
                IsSelected = true,
                IpAddress = "127.0.0.1",
                HostName = "localhost",
                Env = "local",
                Group = "개발",
                Description = "테스트",
                IsEnabled = true,
                OriginalLineNumber = 5
            };

            // Act
            var clone = original.Clone();

            // Assert
            Assert.Equal(original.IsSelected, clone.IsSelected);
            Assert.Equal(original.IpAddress, clone.IpAddress);
            Assert.Equal(original.HostName, clone.HostName);
            Assert.Equal(original.Env, clone.Env);
            Assert.Equal(original.Group, clone.Group);
            Assert.Equal(original.Description, clone.Description);
            Assert.Equal(original.IsEnabled, clone.IsEnabled);
            Assert.Equal(original.OriginalLineNumber, clone.OriginalLineNumber);

            // 독립성 확인
            clone.IpAddress = "192.168.1.1";
            Assert.NotEqual(original.IpAddress, clone.IpAddress);
        }

        [Fact]
        public void HostEntry_PropertyChanged_ShouldFireOnIsEnabledChange()
        {
            // Arrange
            var entry = new HostEntry();
            var propertyChangedFired = false;
            var changedPropertyNames = new List<string>();
            
            entry.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
                if (args.PropertyName != null)
                    changedPropertyNames.Add(args.PropertyName);
            };

            // Act
            entry.IsEnabled = true;

            // Assert
            Assert.True(propertyChangedFired);
            Assert.Contains("IsEnabled", changedPropertyNames);
            Assert.Contains("Status", changedPropertyNames);  // Status도 변경됨
        }

        [Fact]
        public void HostEntry_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var entry = new HostEntry();

            // Assert
            Assert.False(entry.IsSelected);
            Assert.Equal(string.Empty, entry.IpAddress);
            Assert.Equal(string.Empty, entry.HostName);
            Assert.Equal(string.Empty, entry.Env);
            Assert.Equal(string.Empty, entry.Group);
            Assert.Equal(string.Empty, entry.Description);
            Assert.False(entry.IsEnabled);
            Assert.Equal(0, entry.OriginalLineNumber);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldBeFalse_WhenNewlyCreated()
        {
            // Arrange & Act
            var entry = new HostEntry();

            // Assert
            Assert.False(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldBeTrue_WhenPropertyChangedAfterMarkAsClean()
        {
            // Arrange
            var entry = new HostEntry
            {
                IpAddress = "127.0.0.1",
                HostName = "localhost",
                Env = "local",
                Group = "개발"
            };
            entry.MarkAsClean();

            // Act
            entry.IpAddress = "192.168.1.1";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldBeFalse_WhenPropertyRevertedToOriginal()
        {
            // Arrange
            var entry = new HostEntry
            {
                IpAddress = "127.0.0.1",
                HostName = "localhost"
            };
            entry.MarkAsClean();

            // Act
            entry.IpAddress = "192.168.1.1";  // 변경
            entry.IpAddress = "127.0.0.1";    // 원래대로 복구

            // Assert
            Assert.False(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_MarkAsClean_ShouldResetDirtyAndNewFlags()
        {
            // Arrange
            var entry = new HostEntry
            {
                IpAddress = "127.0.0.1",
                HostName = "localhost",
                IsNew = true
            };

            // Act
            entry.MarkAsClean();

            // Assert
            Assert.False(entry.IsDirty);
            Assert.False(entry.IsNew);
        }

        [Fact]
        public void HostEntry_IsNew_ShouldAlwaysSetDirtyTrue()
        {
            // Arrange
            var entry = new HostEntry
            {
                IsNew = true
            };

            // Act
            entry.IpAddress = "127.0.0.1";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldDetectHostNameChange()
        {
            // Arrange
            var entry = new HostEntry
            {
                IpAddress = "127.0.0.1",
                HostName = "localhost"
            };
            entry.MarkAsClean();

            // Act
            entry.HostName = "newhost";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldDetectEnvChange()
        {
            // Arrange
            var entry = new HostEntry { Env = "local" };
            entry.MarkAsClean();

            // Act
            entry.Env = "prod";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldDetectGroupChange()
        {
            // Arrange
            var entry = new HostEntry { Group = "개발" };
            entry.MarkAsClean();

            // Act
            entry.Group = "운영";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldDetectDescriptionChange()
        {
            // Arrange
            var entry = new HostEntry { Description = "테스트 설명" };
            entry.MarkAsClean();

            // Act
            entry.Description = "변경된 설명";

            // Assert
            Assert.True(entry.IsDirty);
        }

        [Fact]
        public void HostEntry_IsDirty_ShouldDetectIsEnabledChange()
        {
            // Arrange
            var entry = new HostEntry { IsEnabled = false };
            entry.MarkAsClean();

            // Act
            entry.IsEnabled = true;

            // Assert
            Assert.True(entry.IsDirty);
        }
    }
}
