using System.IO;
using Xunit;
using HostManager.Models;
using HostManager.Services;

namespace HostManager.Tests
{
    public class GroupServiceTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly GroupService _service;

        public GroupServiceTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"groups_test_{Guid.NewGuid()}.json");
            _service = new GroupService(_testFilePath);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public void LoadGroups_WithNonExistentFile_ShouldReturnEmptyList()
        {
            // Act
            var result = _service.LoadGroups();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SaveGroups_ThenLoadGroups_ShouldRoundTrip()
        {
            // Arrange
            var groups = new List<HostGroup>
            {
                new() { Name = "개발서버" },
                new() { Name = "운영서버" },
                new() { Name = "테스트서버" }
            };

            // Act
            _service.SaveGroups(groups);
            var loadedGroups = _service.LoadGroups();

            // Assert
            Assert.Equal(3, loadedGroups.Count);
            Assert.Equal("개발서버", loadedGroups[0].Name);
            Assert.Equal("운영서버", loadedGroups[1].Name);
            Assert.Equal("테스트서버", loadedGroups[2].Name);
        }

        [Fact]
        public void SaveGroups_WithEmptyNames_ShouldFilterOut()
        {
            // Arrange
            var groups = new List<HostGroup>
            {
                new() { Name = "개발서버" },
                new() { Name = "" },           // 빈 이름
                new() { Name = "   " },        // 공백만
                new() { Name = "운영서버" }
            };

            // Act
            _service.SaveGroups(groups);
            var loadedGroups = _service.LoadGroups();

            // Assert
            Assert.Equal(2, loadedGroups.Count);
            Assert.Equal("개발서버", loadedGroups[0].Name);
            Assert.Equal("운영서버", loadedGroups[1].Name);
        }

        [Fact]
        public void SaveGroups_WithEmptyList_ShouldSaveEmptyArray()
        {
            // Arrange
            var groups = new List<HostGroup>();

            // Act
            _service.SaveGroups(groups);
            var loadedGroups = _service.LoadGroups();

            // Assert
            Assert.Empty(loadedGroups);
        }

        [Fact]
        public void LoadGroups_WithCorruptedJson_ShouldReturnEmptyList()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "{ invalid json }");

            // Act
            var result = _service.LoadGroups();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SaveGroups_ShouldCreateFileWithCorrectFormat()
        {
            // Arrange
            var groups = new List<HostGroup>
            {
                new() { Name = "그룹1" },
                new() { Name = "그룹2" }
            };

            // Act
            _service.SaveGroups(groups);

            // Assert - 저장 후 다시 로드하여 검증
            var loadedGroups = _service.LoadGroups();
            Assert.Contains(loadedGroups, g => g.Name == "그룹1");
            Assert.Contains(loadedGroups, g => g.Name == "그룹2");
            
            // JSON 배열 형식 확인
            var content = File.ReadAllText(_testFilePath);
            Assert.StartsWith("[", content.Trim());
            Assert.EndsWith("]", content.Trim());
        }
    }
}
