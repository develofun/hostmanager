using System.IO;
using Xunit;
using HostManager.Models;
using HostManager.Services;

namespace HostManager.Tests
{
    public class EnvServiceTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly EnvService _service;

        public EnvServiceTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"envs_test_{Guid.NewGuid()}.json");
            _service = new EnvService(_testFilePath);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public void LoadEnvs_WithNonExistentFile_ShouldReturnDefaultEnvs()
        {
            // Act
            var result = _service.LoadEnvs();

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Contains(result, e => e.Name == "local");
            Assert.Contains(result, e => e.Name == "qa");
            Assert.Contains(result, e => e.Name == "stage");
            Assert.Contains(result, e => e.Name == "prod");
        }

        [Fact]
        public void LoadEnvs_DefaultEnvs_ShouldBeMarkedAsDefault()
        {
            // Act
            var result = _service.LoadEnvs();

            // Assert
            foreach (var env in result)
            {
                Assert.True(env.IsDefault, $"{env.Name} should be marked as default");
            }
        }

        [Fact]
        public void SaveEnvs_ThenLoadEnvs_ShouldRoundTrip()
        {
            // Arrange
            var envs = new List<HostEnv>
            {
                new() { Name = "local", IsDefault = true },
                new() { Name = "dev", IsDefault = false },
                new() { Name = "prod", IsDefault = true }
            };

            // Act
            _service.SaveEnvs(envs);
            var loadedEnvs = _service.LoadEnvs();

            // Assert
            Assert.Equal(3, loadedEnvs.Count);
            Assert.Equal("local", loadedEnvs[0].Name);
            Assert.Equal("dev", loadedEnvs[1].Name);
            Assert.Equal("prod", loadedEnvs[2].Name);
        }

        [Fact]
        public void SaveEnvs_WithCustomEnv_ShouldNotBeMarkedAsDefault()
        {
            // Arrange
            var envs = new List<HostEnv>
            {
                new() { Name = "custom-env", IsDefault = false }
            };

            // Act
            _service.SaveEnvs(envs);
            var loadedEnvs = _service.LoadEnvs();

            // Assert
            var customEnv = loadedEnvs.FirstOrDefault(e => e.Name == "custom-env");
            Assert.NotNull(customEnv);
            Assert.False(customEnv.IsDefault);
        }

        [Fact]
        public void SaveEnvs_WithEmptyNames_ShouldFilterOut()
        {
            // Arrange
            var envs = new List<HostEnv>
            {
                new() { Name = "local" },
                new() { Name = "" },
                new() { Name = "   " },
                new() { Name = "prod" }
            };

            // Act
            _service.SaveEnvs(envs);
            var loadedEnvs = _service.LoadEnvs();

            // Assert
            Assert.Equal(2, loadedEnvs.Count);
        }

        [Fact]
        public void GetDefaultEnvs_ShouldReturnFourDefaults()
        {
            // Act
            var defaults = _service.GetDefaultEnvs();

            // Assert
            Assert.Equal(4, defaults.Count);
            Assert.Contains("local", defaults);
            Assert.Contains("qa", defaults);
            Assert.Contains("stage", defaults);
            Assert.Contains("prod", defaults);
        }

        [Fact]
        public void LoadEnvs_WithCorruptedJson_ShouldReturnDefaultEnvs()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "not valid json");

            // Act
            var result = _service.LoadEnvs();

            // Assert
            Assert.Equal(4, result.Count);  // 기본값 반환
        }
    }
}
