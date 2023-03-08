using Microsoft.Extensions.Logging;
using Moq;
using TradeArt.Domain.Interfaces;
using TradeArt.Infrastructure.Services;
using Xunit;

namespace TradeArt.UnitTest.Services
{
	public class EmitterServiceTests
	{
		[Fact]
		public async Task EmitDataAsync_ShouldCallProcessDataAsync_WithEachDataItem()
		{
			// Arrange
			var data = new List<int> { 1, 2, 3 };
			var cancellationToken = new CancellationToken();
			var mockProcessor = new Mock<IDataProcessorService>();
			var mockLogger = new Mock<ILogger<EmitterService>>();
			var service = new EmitterService(mockProcessor.Object, mockLogger.Object);

			// Act
			await service.EmitDataAsync(data, cancellationToken);

			// Assert
			foreach (var item in data)
			{
				mockProcessor.Verify(p => p.ProcessDataAsync(item, cancellationToken), Times.Once);
			}
		}
	}
}
