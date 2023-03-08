using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TradeArt.Infrastructure.Configuration;
using TradeArt.Infrastructure.Services;
using TradeArt.UnitTest.Helpers;
using Xunit;

namespace TradeArt.UnitTest.Services
{

    public class FileHashServiceTests
    {
        private readonly Mock<ILogger<FileHashService>> _loggerMock;
        private readonly Mock<IOptions<AppSetting>> _appSettingsMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private FileHashService _fileHashService;

        public FileHashServiceTests()
        {
            _loggerMock = new Mock<ILogger<FileHashService>>();
            _appSettingsMock = new Mock<IOptions<AppSetting>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            _appSettingsMock.Setup(x => x.Value).Returns(new AppSetting
            {
                ChunkSize = 1024
            });

            _fileHashService = new FileHashService(
                _loggerMock.Object,
                _appSettingsMock.Object,
                _httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task CalculateFileHashFromRemoteServerAsync_ShouldCalculateHashFromRemoteFile()
        {
            // Arrange
            var expectedHash = "f29bc64a9d3732b4b9035125fdb3285f5b6455778edca72414671e0ca3b2e0de";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a test file."));
            var httpContent = new StreamContent(stream);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = httpContent
            };

            var mockHandler = new TestHttpMessageHandler(async (request, token) => response);

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(mockHandler));

            // Act
            var hash = await _fileHashService.CalculateFileHashFromRemoteServerAsync("https://test.com/file.txt",
                CancellationToken.None);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().Be(expectedHash);
        }

        [Fact]
        public async Task CalculateFileFromLocalPathAsync_ShouldCalculateHashFromLocalFile()
        {
            // Arrange
            var expectedHash = "f29bc64a9d3732b4b9035125fdb3285f5b6455778edca72414671e0ca3b2e0de";

            var tempFilePath = Path.GetTempFileName();

            await File.WriteAllTextAsync(tempFilePath, "This is a test file.");

            // Act
            var hash = await _fileHashService.CalculateFileFromLocalPathAsync(tempFilePath, CancellationToken.None);

            // Assert
            hash.Should().Be(expectedHash);

        }

        [Fact]
        public async Task CalculateHash_ShouldCalculateHashFromFileStream()
        {
            // Arrange
            var expectedHash = "f29bc64a9d3732b4b9035125fdb3285f5b6455778edca72414671e0ca3b2e0de";

            var tempFilePath = Path.GetTempFileName();

            await File.WriteAllTextAsync(tempFilePath, "This is a test file.");

            await using var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);

            // Act
            var hash = await _fileHashService.CalculateHash(tempFilePath, CancellationToken.None);

            // Assert
            hash.Should().Be(expectedHash);

        }
        [Fact]
        public async Task CalculateFileHashFromRemoteServerAsync_Should_Log_Exception()
        {
            // Arrange
            const string url = "https://www.example.com/file.txt";
            var cancellationToken = CancellationToken.None;

            var mockHandler = new TestHttpMessageHandler(async (request, token) => throw new HttpRequestException());


            var httpClient = new HttpClient(mockHandler);

            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _fileHashService = new FileHashService(
                _loggerMock.Object,
                _appSettingsMock.Object,
                _httpClientFactoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _fileHashService.CalculateFileHashFromRemoteServerAsync(url, cancellationToken));

            _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }

        [Fact]
        public async Task CalculateFileFromLocalPathAsync_Should_Log_Exception()
        {
            // Arrange
            var path = "C:/Some/File/That/Doesnt/Exist.txt";
            var cancellationToken = CancellationToken.None;

            // Act and Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _fileHashService.CalculateFileFromLocalPathAsync(path, cancellationToken));

            _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }
    }
}
