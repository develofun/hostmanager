using System.IO;
using Xunit;
using HostManager.Services;

namespace HostManager.Tests
{
    public class HostsFileServiceTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly HostsFileService _service;

        public HostsFileServiceTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"hosts_test_{Guid.NewGuid()}.txt");
            _service = new HostsFileService(_testFilePath);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        #region IP 유효성 검사 테스트

        [Theory]
        [InlineData("127.0.0.1", true)]
        [InlineData("192.168.1.1", true)]
        [InlineData("255.255.255.255", true)]
        [InlineData("0.0.0.0", true)]
        [InlineData("10.0.0.1", true)]
        [InlineData("256.1.1.1", false)]       // 256은 범위 초과
        [InlineData("192.168.1", false)]        // 불완전한 IP
        [InlineData("192.168.1.1.1", false)]    // IP 형식 초과
        [InlineData("abc.def.ghi.jkl", false)]  // 문자열
        [InlineData("", false)]                  // 빈 문자열
        [InlineData(null, false)]               // null
        [InlineData("  ", false)]               // 공백
        [InlineData("192.168.1.1 ", true)]      // 끝에 공백 (trim 처리)
        [InlineData(" 192.168.1.1", true)]      // 앞에 공백 (trim 처리)
        public void IsValidIpAddress_ShouldValidateCorrectly(string ip, bool expected)
        {
            // Act
            var result = HostsFileService.IsValidIpAddress(ip);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region 호스트명 유효성 검사 테스트

        [Theory]
        [InlineData("localhost", true)]
        [InlineData("example.com", true)]
        [InlineData("sub.example.com", true)]
        [InlineData("my-server", true)]
        [InlineData("server1", true)]
        [InlineData("a", true)]                  // 단일 문자
        [InlineData("api.dev.example.com", true)]
        [InlineData("-invalid", false)]          // 하이픈으로 시작
        [InlineData("invalid-", false)]          // 하이픈으로 끝
        [InlineData("", false)]                  // 빈 문자열
        [InlineData(null, false)]               // null
        [InlineData("  ", false)]               // 공백
        [InlineData("host name", false)]        // 공백 포함
        public void IsValidHostName_ShouldValidateCorrectly(string hostName, bool expected)
        {
            // Act
            var result = HostsFileService.IsValidHostName(hostName);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region 호스트 파일 파싱 테스트

        [Fact]
        public void LoadHosts_WithEmptyFile_ShouldReturnEmptyList()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "");

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void LoadHosts_WithNonExistentFile_ShouldReturnEmptyList()
        {
            // Act (파일이 없는 상태)
            var result = _service.LoadHosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void LoadHosts_WithSimpleEntry_ShouldParseCorrectly()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "127.0.0.1 localhost");

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Single(result);
            Assert.Equal("127.0.0.1", result[0].IpAddress);
            Assert.Equal("localhost", result[0].HostName);
            Assert.True(result[0].IsEnabled);
        }

        [Fact]
        public void LoadHosts_WithDisabledEntry_ShouldParseCorrectly()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "# 127.0.0.1 localhost");

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Single(result);
            Assert.Equal("127.0.0.1", result[0].IpAddress);
            Assert.Equal("localhost", result[0].HostName);
            Assert.False(result[0].IsEnabled);
        }

        [Fact]
        public void LoadHosts_WithMetadata_ShouldParseCorrectly()
        {
            // Arrange
            File.WriteAllText(_testFilePath, 
                "127.0.0.1 localhost # [Env:local] [Group:개발서버] [Desc:로컬 개발 서버]");

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Single(result);
            Assert.Equal("127.0.0.1", result[0].IpAddress);
            Assert.Equal("localhost", result[0].HostName);
            Assert.Equal("local", result[0].Env);
            Assert.Equal("개발서버", result[0].Group);
            Assert.Equal("로컬 개발 서버", result[0].Description);
            Assert.True(result[0].IsEnabled);
        }

        [Fact]
        public void LoadHosts_WithPartialMetadata_ShouldParseCorrectly()
        {
            // Arrange
            File.WriteAllText(_testFilePath, 
                "192.168.1.100 api.server.com # [Env:prod] [Desc:프로덕션 API]");

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Single(result);
            Assert.Equal("192.168.1.100", result[0].IpAddress);
            Assert.Equal("api.server.com", result[0].HostName);
            Assert.Equal("prod", result[0].Env);
            Assert.Empty(result[0].Group);  // Group이 없음
            Assert.Equal("프로덕션 API", result[0].Description);
        }

        [Fact]
        public void LoadHosts_WithMultipleEntries_ShouldParseAll()
        {
            // Arrange
            var content = @"127.0.0.1 localhost
192.168.1.1 server1 # [Env:dev]
# 10.0.0.1 disabled-server
192.168.1.2 server2 # [Group:웹서버]";
            File.WriteAllText(_testFilePath, content);

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Equal(4, result.Count);
            Assert.True(result[0].IsEnabled);
            Assert.True(result[1].IsEnabled);
            Assert.False(result[2].IsEnabled);  // disabled
            Assert.True(result[3].IsEnabled);
        }

        [Fact]
        public void LoadHosts_WithCommentsAndEmptyLines_ShouldIgnoreNonHostLines()
        {
            // Arrange
            var content = @"# This is a comment
# Another comment

127.0.0.1 localhost

# More comments here
192.168.1.1 server";
            File.WriteAllText(_testFilePath, content);

            // Act
            var result = _service.LoadHosts();

            // Assert
            Assert.Equal(2, result.Count);  // IP가 있는 라인만
        }

        #endregion

        #region 호스트 파일 저장 테스트

        [Fact]
        public void SaveHosts_WithSingleEntry_ShouldSaveCorrectly()
        {
            // Arrange
            var entries = new List<HostManager.Models.HostEntry>
            {
                new() { IpAddress = "127.0.0.1", HostName = "localhost", IsEnabled = true }
            };

            // Act
            _service.SaveHosts(entries);

            // Assert
            var content = File.ReadAllText(_testFilePath);
            Assert.Contains("127.0.0.1 localhost", content);
        }

        [Fact]
        public void SaveHosts_WithDisabledEntry_ShouldAddHashPrefix()
        {
            // Arrange
            var entries = new List<HostManager.Models.HostEntry>
            {
                new() { IpAddress = "127.0.0.1", HostName = "localhost", IsEnabled = false }
            };

            // Act
            _service.SaveHosts(entries);

            // Assert
            var content = File.ReadAllText(_testFilePath);
            Assert.Contains("# 127.0.0.1 localhost", content);
        }

        [Fact]
        public void SaveHosts_WithMetadata_ShouldSaveAllMetadata()
        {
            // Arrange
            var entries = new List<HostManager.Models.HostEntry>
            {
                new() 
                { 
                    IpAddress = "192.168.1.100", 
                    HostName = "api.server.com", 
                    Env = "prod",
                    Group = "API서버",
                    Description = "프로덕션 API 서버",
                    IsEnabled = true 
                }
            };

            // Act
            _service.SaveHosts(entries);

            // Assert
            var content = File.ReadAllText(_testFilePath);
            Assert.Contains("192.168.1.100 api.server.com", content);
            Assert.Contains("[Env:prod]", content);
            Assert.Contains("[Group:API서버]", content);
            Assert.Contains("[Desc:프로덕션 API 서버]", content);
        }

        [Fact]
        public void SaveHosts_ThenLoadHosts_ShouldRoundTrip()
        {
            // Arrange
            var entries = new List<HostManager.Models.HostEntry>
            {
                new() 
                { 
                    IpAddress = "127.0.0.1", 
                    HostName = "localhost", 
                    Env = "local",
                    Group = "개발",
                    Description = "로컬 서버",
                    IsEnabled = true 
                },
                new() 
                { 
                    IpAddress = "192.168.1.1", 
                    HostName = "server.com", 
                    Env = "prod",
                    Group = "",
                    Description = "운영 서버",
                    IsEnabled = false 
                }
            };

            // Act
            _service.SaveHosts(entries);
            var loadedEntries = _service.LoadHosts();

            // Assert
            Assert.Equal(2, loadedEntries.Count);
            
            Assert.Equal("127.0.0.1", loadedEntries[0].IpAddress);
            Assert.Equal("localhost", loadedEntries[0].HostName);
            Assert.Equal("local", loadedEntries[0].Env);
            Assert.Equal("개발", loadedEntries[0].Group);
            Assert.Equal("로컬 서버", loadedEntries[0].Description);
            Assert.True(loadedEntries[0].IsEnabled);

            Assert.Equal("192.168.1.1", loadedEntries[1].IpAddress);
            Assert.Equal("server.com", loadedEntries[1].HostName);
            Assert.Equal("prod", loadedEntries[1].Env);
            Assert.Equal("운영 서버", loadedEntries[1].Description);
            Assert.False(loadedEntries[1].IsEnabled);
        }

        #endregion
    }
}
