using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using TradeArt.Infrastructure.Repositories;
using TradeArt.Infrastructure.Services;
using Xunit;

namespace TradeArt.UnitTest.Services
{
    public class InvertTextServiceTests
    {
        private Mock<IInvertTextRepository> _repositoryMock;
        private Mock<ILogger<InvertTextService>> _loggerMock;
        private InvertTextService _service;

        public InvertTextServiceTests()
        {
            _repositoryMock = new Mock<IInvertTextRepository>();
            _loggerMock = new Mock<ILogger<InvertTextService>>();
            _service = new InvertTextService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void InvertText_InputIsNull_ThrowsArgumentNullExceptionAndLogsError()
        {
            // Arrange
            string text = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.InvertText(text));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("Input text is null.", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                )
            );
        }

        [Fact]
        public void InvertText_InversionSucceeds_ReturnsInvertedTextAndLogsInformation()
        {
            // Arrange
            string text = "Hello, world!";
            string invertedText = "!dlrow ,olleH";
            _repositoryMock.Setup(x => x.InvertText(It.IsAny<StringBuilder>())).Returns(invertedText);

            // Act
            string result = _service.InvertText(text);

            // Assert
            Assert.Equal(invertedText, result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Inverted text: {invertedText}", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );
        }

        [Fact]
        public void InvertText_InversionFails_ThrowsAndLogsError()
        {
            // Arrange
            string text = "Hello, world!";
            _repositoryMock.Setup(x =>
                    x.InvertText(It.IsAny<StringBuilder>()))
                .Throws<Exception>();

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.InvertText(text));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );
        }
    }
}
